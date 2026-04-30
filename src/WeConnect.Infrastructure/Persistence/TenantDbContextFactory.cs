namespace WeConnect.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WeConnect.Domain.Interfaces;

public class TenantDbContextFactory(
    ITenantConnectionRepository connRepo,
    IMemoryCache cache)
{
    public async Task<TenantDbContext> CreateAsync(
        string slug, CancellationToken ct = default)
    {
        var connString = await GetConnectionStringAsync(slug, ct);

        var opts = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(connString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new TenantDbContext(opts);
    }

    private async Task<string> GetConnectionStringAsync(
        string slug, CancellationToken ct)
    {
        var cacheKey = $"tenant_conn:{slug}";

        if (cache.TryGetValue(cacheKey, out string? cached) && cached != null)
            return cached;

        var conn = await connRepo.GetBySlugAsync(slug, ct)
            ?? throw new InvalidOperationException(
                   $"No connection record found for tenant '{slug}'");

        var connString = conn.BuildConnectionString();

        // Cache for 5 min — shorter than config cache intentionally
        cache.Set(cacheKey, connString, TimeSpan.FromMinutes(5));

        return connString;
    }

    // Call this from admin when tenant is suspended/deleted
    public void InvalidateCache(string slug) =>
        cache.Remove($"tenant_conn:{slug}");
}