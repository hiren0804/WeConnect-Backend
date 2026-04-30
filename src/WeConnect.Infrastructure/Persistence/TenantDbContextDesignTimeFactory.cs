namespace WeConnect.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

// Used only by EF CLI for migrations — not used at runtime
// Run: dotnet ef migrations add <Name> --context TenantDbContext
public class TenantDbContextDesignTimeFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        // Reads from env vars so you don't need to hardcode credentials
        // Set these before running migrations:
        //   POSTGRES_HOST, POSTGRES_PORT, POSTGRES_USERNAME, POSTGRES_PASSWORD
        var host     = Environment.GetEnvironmentVariable("POSTGRES_HOST")     ?? "localhost";
        var port     = Environment.GetEnvironmentVariable("POSTGRES_PORT")     ?? "5433";
        var username = Environment.GetEnvironmentVariable("POSTGRES_USERNAME") ?? "hiren";
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "hiren@1234";

        var opts = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(
                $"Host={host};Port={port};Database=weconnect_base;" +
                $"Username={username};Password={password}")
            .UseSnakeCaseNamingConvention()
            .Options;

        return new TenantDbContext(opts);
    }
}
