using Microsoft.Extensions.Caching.Memory;
using WeConnect.Application.DTOs;
using WeConnect.Domain.Entities;
using WeConnect.Application.Common.Interfaces;

namespace WeConnect.Application.Services;

public class TenantService(ITenantRepository repo, IMemoryCache cache) : ITenantService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);

    public async Task<TenantConfigDto?> GetConfigAsync(
        string slug, CancellationToken ct = default)
    {
        var key = $"tenant_cfg:{slug}";

        if (cache.TryGetValue(key, out TenantConfigDto? cached))
            return cached;

        var tenant = await repo.GetBySlugAsync(slug, ct);
        if (tenant is null) return null;

        var dto = MapToDto(tenant);
        cache.Set(key, dto, CacheTtl);
        return dto;
    }

    // Used by middleware
    public async Task<Tenant?> GetTenantAsync(
        string slug, CancellationToken ct = default)
        => await repo.GetBySlugAsync(slug, ct);

    public async Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken ct = default)
        => await repo.GetAllAsync(ct);

    public async Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await repo.GetBySlugAsync(slug, ct);

    public async Task<Tenant> UpdateAsync(Tenant tenant, CancellationToken ct = default)
    {
        repo.Update(tenant);
        await repo.SaveChangesAsync(ct);
        InvalidateCache(tenant.Slug);
        return tenant;
    }

    public async Task<bool> DeleteAsync(string slug, CancellationToken ct = default)
    {
        var tenant = await GetBySlugAsync(slug, ct);
        if (tenant is null) return false;
        repo.Delete(tenant);
        await repo.SaveChangesAsync(ct);
        InvalidateCache(slug);
        return true;
    }

    public void InvalidateCache(string slug)
        => cache.Remove($"tenant_cfg:{slug}");

    private static TenantConfigDto MapToDto(Tenant t) => new(
        Client: new ClientDto(t.Name, t.Slug, t.Domain),
        Template: new TemplateDto(t.TemplateId),
        Modules: t.Modules
            .Where(m => m.IsEnabled)
            .OrderBy(m => m.DisplayOrder)
            .Select(m => new ModuleDto(
                Id: m.ModuleKey,
                Enabled: m.IsEnabled,
                Pages: m.Pages.Select(p => new PageDto(
                    Id: p.PageKey,
                    Type: p.PageType,
                    Route: p.Route,
                    Card: p.CardComponent,
                    Widgets: p.Widgets
                        .Where(w => w.IsVisible)
                        .OrderBy(w => w.DisplayOrder)
                        .Select(w => new WidgetDto(
                            w.WidgetKey, w.WidgetType,
                            w.Col, w.Height,
                            w.DisplayOrder, w.IsVisible))
                ))
            ))
    );
}



