namespace WeConnect.API.Middleware;

using WeConnect.Application.Common.Interfaces;
using WeConnect.Infrastructure.Persistence;

public class TenantMiddleware(RequestDelegate next, IConfiguration config)
{
    // Routes that don't need a tenant (admin, health, swagger)
    private static readonly string[] BypassPrefixes =
        ["/api/admin", "/health", "/swagger", "/api/health"];

    public async Task InvokeAsync(
        HttpContext ctx,
        ITenantService tenantService,
        TenantDbContextFactory dbFactory)
    {
        // ── Skip tenant resolution for admin/infra routes ─────────────
        var path = ctx.Request.Path.Value ?? "";
        if (BypassPrefixes.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await next(ctx);
            return;
        }

        var slug = ResolveSlug(ctx);

        // ── No slug resolved at all ────────────────────────────────────
        if (string.IsNullOrWhiteSpace(slug))
        {
            ctx.Response.StatusCode = 400;
            await ctx.Response.WriteAsJsonAsync(
                new { error = "Cannot resolve tenant. Pass X-Tenant-Slug header or use subdomain." });
            return;
        }

        // ── Tenant lookup ──────────────────────────────────────────────
        var tenant = await tenantService.GetTenantAsync(slug);
        if (tenant is null)
        {
            ctx.Response.StatusCode = 404;
            await ctx.Response.WriteAsJsonAsync(
                new { error = "Tenant not found", slug });
            return;
        }

        if (tenant.Status != "active")
        {
            ctx.Response.StatusCode = 503;
            await ctx.Response.WriteAsJsonAsync(
                new { error = "Tenant is not ready", status = tenant.Status });
            return;
        }

        var tenantConfig = await tenantService.GetConfigAsync(slug);

        // ── Create tenant DB context ───────────────────────────────────
        TenantDbContext tenantDb;
        try
        {
            tenantDb = await dbFactory.CreateAsync(slug);
        }
        catch (InvalidOperationException)
        {
            ctx.Response.StatusCode = 503;
            await ctx.Response.WriteAsJsonAsync(
                new { error = "Tenant database unavailable" });
            return;
        }

        ctx.Items["TenantSlug"]   = slug;
        ctx.Items["TenantConfig"] = tenantConfig;
        ctx.Items["TenantId"]     = tenant.Id;
        ctx.Items["TenantDb"]     = tenantDb;

        await next(ctx);

        await tenantDb.DisposeAsync();
    }

    private string ResolveSlug(HttpContext ctx)
    {
        var host = ctx.Request.Host.Host;

        // ── Real domain → extract subdomain ───────────────────────────
        if (host is not ("localhost" or "127.0.0.1"))
        {
            var parts = host.Split('.');
            // e.g. acme.weconnect.com → parts[0] = "acme"
            return parts.Length >= 3 ? parts[0] : string.Empty;
        }

        // ── Local dev: prefer explicit header ─────────────────────────
        var header = ctx.Request.Headers["X-Tenant-Slug"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(header)) return header;

        // ── Local dev: fall back to config (not hardcoded) ────────────
        return config["Provisioning:DefaultTenantSlug"] ?? string.Empty;
    }
}