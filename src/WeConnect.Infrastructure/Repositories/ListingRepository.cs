using Microsoft.EntityFrameworkCore;
using WeConnect.Domain.Entities;
using WeConnect.Domain.Interfaces;
using WeConnect.Infrastructure.Persistence;

namespace WeConnect.Infrastructure.Repositories;

public class ListingRepository(TenantDbContext db) : IListingRepository
{
    public async Task<IEnumerable<ContentItem>> GetAllAsync(
        Guid tenantId, CancellationToken ct = default) =>
        await db.ContentItems
            .Where(c => c.TenantId == tenantId)
            .OrderByDescending(c => c.PublishedAt)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<IEnumerable<ContentItem>> GetByTypeAsync(
        Guid tenantId, string contentType, CancellationToken ct = default) =>
        await db.ContentItems
            .Where(c => c.TenantId == tenantId && c.ContentType == contentType)
            .OrderByDescending(c => c.PublishedAt)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<ContentItem?> GetByIdAsync(
        Guid id, Guid tenantId, CancellationToken ct = default) =>
        await db.ContentItems
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId, ct);

    public async Task<ContentItem> CreateAsync(
        ContentItem item, CancellationToken ct = default)
    {
        db.ContentItems.Add(item);
        await db.SaveChangesAsync(ct);
        return item;
    }

    public async Task<ContentItem> UpdateAsync(
        ContentItem item, CancellationToken ct = default)
    {
        db.Entry(item).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
        return item;
    }

    public async Task<bool> DeleteAsync(
        Guid id, Guid tenantId, CancellationToken ct = default)
    {
        var item = await db.ContentItems
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId, ct);
        if (item is null) return false;

        db.ContentItems.Remove(item);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ExistsAsync(
        Guid id, Guid tenantId, CancellationToken ct = default) =>
        await db.ContentItems
            .AnyAsync(c => c.Id == id && c.TenantId == tenantId, ct);
}
