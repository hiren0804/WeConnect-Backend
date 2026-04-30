using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using WeConnect.Domain.Entities;
using WeConnect.Infrastructure.Persistence;

namespace WeConnect.Infrastructure.Services;

public class ProvisioningService(MasterDbContext master, IConfiguration config)
{

    private static readonly Regex SlugRegex =
        new(@"^[a-z0-9][a-z0-9\-]{1,30}[a-z0-9]$", RegexOptions.Compiled);

    private static readonly HashSet<string> Reserved =
        ["base", "master", "admin", "api", "www", "mail", "test"];

    // ── Validation ────────────────────────────────────────────────────
    public async Task<(bool success, string error)> ValidateSlugAsync(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return (false, "Slug cannot be empty");

        if (!SlugRegex.IsMatch(slug))
            return (false, "Slug must be 3-32 lowercase alphanumeric chars or hyphens");

        if (Reserved.Contains(slug))
            return (false, $"'{slug}' is a reserved name");

        var exists = await master.Tenants.AnyAsync(t => t.Slug == slug);
        if (exists)
            return (false, $"Slug '{slug}' is already taken");

        return (true, string.Empty);
    }

    // ── Called by AdminController ─────────────────────────────────────
    public async Task<Tenant> ProvisionAsync(
        string name, string slug, string domain,
        string templateId, CancellationToken ct = default)
    {
        var (valid, err) = await ValidateSlugAsync(slug);
        if (!valid) throw new ProvisioningException(err, new Exception(err));

        var tenant = new Tenant
        {
            Name = name,
            Slug = slug,
            Domain = domain,
            TemplateId = templateId,
            IsActive = false,
            Status = "provisioning"
        };
        master.Tenants.Add(tenant);
        await master.SaveChangesAsync(ct);

        await ProvisionTenantDbAsync(tenant, ct);

        tenant.IsActive = true;
        tenant.Status = "active";
        await master.SaveChangesAsync(ct);

        return tenant;
    }

    // ── Called by both AdminController AND DatabaseSeeder ─────────────
    public async Task ProvisionTenantDbAsync(Tenant tenant, CancellationToken ct = default)
    {
        var dbName = $"weconnect_{tenant.Slug.Replace("-", "_")}";

        var pgHost = config["Postgres:Host"] ?? "localhost";
        var pgPort = int.Parse(config["Postgres:Port"] ?? "5432");
        var pgUser = config["Postgres:Username"] ?? "postgres";
        var pgPass = config["Postgres:Password"] ?? "";

        // ── Step 1: Verify weconnect_base exists before attempting clone ──
        var baseExists = await CheckDatabaseExistsAsync(pgHost, pgPort, pgUser, pgPass, "weconnect_base", ct);
        if (!baseExists)
        {
            await LogAsync(tenant.Id, "clone_db", "failed",
                "weconnect_base does not exist. Run: dotnet ef database update --context TenantDbContext");
            await RollbackTenantAsync(tenant, dbName, ct);
            throw new ProvisioningException(
                "weconnect_base template DB not found. Create it first.",
                new InvalidOperationException("weconnect_base missing"));
        }

        // ── Step 2: Check target DB doesn't already exist ────────────
        var targetExists = await CheckDatabaseExistsAsync(pgHost, pgPort, pgUser, pgPass, dbName, ct);
        if (targetExists)
        {
            await LogAsync(tenant.Id, "clone_db", "failed",
                $"Database '{dbName}' already exists");
            await RollbackTenantAsync(tenant, dbName, ct);
            throw new ProvisioningException(
                $"Database '{dbName}' already exists. Drop it first or use a different slug.",
                new InvalidOperationException($"{dbName} already exists"));
        }

        // ── Step 3: Clone base DB ─────────────────────────────────────
        await LogAsync(tenant.Id, "clone_db", "started");
        try
        {
            await CloneBaseDatabaseAsync(dbName, pgHost, pgPort, pgUser, pgPass, ct);
            await LogAsync(tenant.Id, "clone_db", "success");
        }
        catch (Exception ex)
        {
            await LogAsync(tenant.Id, "clone_db", "failed", ex.Message);
            // Rollback: remove tenant row — slug is free to retry
            await RollbackTenantAsync(tenant, dbName, ct);
            throw new ProvisioningException(
                $"Failed to clone weconnect_base → {dbName}: {ex.Message}", ex);
        }

        // ── Step 4: Store connection record ───────────────────────────
        var alreadyHasConn = await master.Connections
            .AnyAsync(c => c.TenantId == tenant.Id, ct);

        if (!alreadyHasConn)
        {
            master.Connections.Add(new TenantConnection
            {
                TenantId = tenant.Id,
                Host = pgHost,
                Port = pgPort,
                Database = dbName,
                Username = pgUser,
                Password = pgPass
            });
            await master.SaveChangesAsync(ct);
        }

        // ── Step 5: Run EF migrations on new tenant DB ────────────────
        await LogAsync(tenant.Id, "run_migrations", "started");
        try
        {
            var connStr = $"Host={pgHost};Port={pgPort};Database={dbName};" +
                          $"Username={pgUser};Password={pgPass}";
            await RunMigrationsAsync(connStr, ct);
            await LogAsync(tenant.Id, "run_migrations", "success");
        }
        catch (Exception ex)
        {
            await LogAsync(tenant.Id, "run_migrations", "failed", ex.Message);
            tenant.Status = "migration_failed";
            await master.SaveChangesAsync(ct);
            throw new ProvisioningException(
                $"Migrations failed on {dbName}: {ex.Message}", ex);
        }

        // ── Step 6: Scaffold frontend folder (non-fatal) ─────────────
        await LogAsync(tenant.Id, "scaffold_folder", "started");
        try
        {
            ScaffoldClientFolder(config, tenant.Slug, tenant.TemplateId);
            await LogAsync(tenant.Id, "scaffold_folder", "success");
        }
        catch (Exception ex)
        {
            // Non-fatal — DB is ready, folder can be manually created
            await LogAsync(tenant.Id, "scaffold_folder", "failed", ex.Message);
            Console.WriteLine($"[Provisioning] Folder scaffold failed for '{tenant.Slug}': {ex.Message}");
        }
    }

    // ── Rollback: removes tenant row + cleans up partial DB ──────────
    private async Task RollbackTenantAsync(
        Tenant tenant, string dbName, CancellationToken ct)
    {
        try
        {
            // Remove connection record if it was inserted
            var conn = await master.Connections
                .FirstOrDefaultAsync(c => c.TenantId == tenant.Id, ct);
            if (conn is not null) master.Connections.Remove(conn);

            // Remove tenant row so slug is free to retry
            var existing = await master.Tenants
                .FirstOrDefaultAsync(t => t.Id == tenant.Id, ct);
            if (existing is not null) master.Tenants.Remove(existing);

            await master.SaveChangesAsync(ct);

            // Drop partial DB if it was created
            await DropDatabaseIfExistsAsync(dbName, ct);

            Console.WriteLine($"[Provisioning] Rolled back tenant '{tenant.Slug}' cleanly.");
        }
        catch (Exception rollbackEx)
        {
            // Rollback itself failed — log but don't rethrow
            Console.WriteLine($"[Provisioning] Rollback failed for '{tenant.Slug}': {rollbackEx.Message}");
        }
    }

    // ── Checks if a Postgres DB exists ────────────────────────────────
    private static async Task<bool> CheckDatabaseExistsAsync(
        string host, int port, string user, string pass,
        string dbName, CancellationToken ct)
    {
        var sysConn = $"Host={host};Port={port};Database=postgres;" +
                      $"Username={user};Password={pass}";

        await using var conn = new NpgsqlConnection(sysConn);
        await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT 1 FROM pg_database WHERE datname = @name";
        cmd.Parameters.AddWithValue("@name", dbName);

        var result = await cmd.ExecuteScalarAsync(ct);
        return result is not null;
    }

    // ── Clones weconnect_base → dbName ────────────────────────────────
    private static async Task CloneBaseDatabaseAsync(
        string dbName, string host, int port,
        string user, string pass, CancellationToken ct)
    {
        var sysConn = $"Host={host};Port={port};Database=postgres;" +
                      $"Username={user};Password={pass}";

        await using var conn = new NpgsqlConnection(sysConn);
        await conn.OpenAsync(ct);

        // Kill active connections to weconnect_base (required for TEMPLATE clone)
        await using var killCmd = conn.CreateCommand();
        killCmd.CommandText = """
            SELECT pg_terminate_backend(pid)
            FROM   pg_stat_activity
            WHERE  datname = 'weconnect_base'
            AND    pid <> pg_backend_pid()
            """;
        await killCmd.ExecuteNonQueryAsync(ct);

        // Small delay — give Postgres time to process terminations
        await Task.Delay(200, ct);

        // Clone
        await using var cloneCmd = conn.CreateCommand();
        cloneCmd.CommandText = $"CREATE DATABASE \"{dbName}\" TEMPLATE weconnect_base";
        await cloneCmd.ExecuteNonQueryAsync(ct);
    }

    // ── Drops a DB if it exists (used in rollback) ────────────────────
    private async Task DropDatabaseIfExistsAsync(string dbName, CancellationToken ct)
    {
        var pgHost = config["Postgres:Host"] ?? "localhost";
        var pgPort = int.Parse(config["Postgres:Port"] ?? "5432");
        var pgUser = config["Postgres:Username"] ?? "postgres";
        var pgPass = config["Postgres:Password"] ?? "";

        var sysConn = $"Host={pgHost};Port={pgPort};Database=postgres;" +
                      $"Username={pgUser};Password={pgPass}";

        await using var conn = new NpgsqlConnection(sysConn);
        await conn.OpenAsync(ct);

        // Kill connections first
        await using var killCmd = conn.CreateCommand();
        killCmd.CommandText = $"""
            SELECT pg_terminate_backend(pid)
            FROM   pg_stat_activity
            WHERE  datname = '{dbName}'
            AND    pid <> pg_backend_pid()
            """;
        await killCmd.ExecuteNonQueryAsync(ct);

        await using var dropCmd = conn.CreateCommand();
        dropCmd.CommandText = $"DROP DATABASE IF EXISTS \"{dbName}\"";
        await dropCmd.ExecuteNonQueryAsync(ct);
    }

    // ── Run EF migrations on tenant DB ───────────────────────────────
    private static async Task RunMigrationsAsync(string connString, CancellationToken ct)
    {
        var opts = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(connString)
            .UseSnakeCaseNamingConvention()
            .Options;

        await using var db = new TenantDbContext(opts);
        await db.Database.MigrateAsync(ct);
    }

    // ── Scaffold frontend/clients/{slug}/ folder ──────────────────────
    private static void ScaffoldClientFolder(IConfiguration config, string slug, string templateId)
    {

        // var root         = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../.."));
        // var clientsDir   = Path.Combine(root, "frontend", "clients");
        // var clientFolder = Path.Combine(clientsDir, slug);
    var clientsDir   = config["Provisioning:FrontendClientsPath"]
        ?? throw new InvalidOperationException(
               "Provisioning:FrontendClientsPath is not set in appsettings.json");

    var clientFolder = Path.Combine(clientsDir, slug);

    if (Directory.Exists(clientFolder))
        throw new InvalidOperationException(
            $"Client folder already exists: {clientFolder}");

    Directory.CreateDirectory(clientFolder);

    File.WriteAllText(Path.Combine(clientFolder, "theme.json"), $$"""
    {
        "slug": "{{slug}}",
        "templateId": "{{templateId}}",
        "colors": {
            "primary": "#000000",
            "secondary": "#ffffff",
            "accent": "#0066cc"
        },
        "fonts": {
            "heading": "Inter",
            "body": "Inter"
        }
    }
    """);

    File.WriteAllText(Path.Combine(clientFolder, "overrides.ts"), """
    // Component overrides for this client
    export const overrides = {};
    """);

    var baseLogo = Path.Combine(clientsDir, "_base", "logo.png");
    var destLogo = Path.Combine(clientFolder, "logo.png");
    if (File.Exists(baseLogo)) File.Copy(baseLogo, destLogo);
    }

    // ── Audit log helper ──────────────────────────────────────────────
    private async Task LogAsync(
        Guid tenantId, string step, string status, string? error = null)
    {
        master.ProvisioningLogs.Add(new ProvisioningLog
        {
            TenantId = tenantId,
            Step = step,
            Status = status,
            Error = error
        });
        await master.SaveChangesAsync();
    }
}

public class ProvisioningException(string message, Exception inner)
    : Exception(message, inner);