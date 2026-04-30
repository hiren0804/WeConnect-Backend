namespace WeConnect.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WeConnect.Domain.Entities;
using WeConnect.Domain.Interfaces;
using WeConnect.Infrastructure.Persistence;

public class ContentRepository(TenantDbContext db) : IContentRepository
{
    // TenantId filter is a safety guard — in a per-tenant DB all rows belong
    // to the same tenant, but filtering prevents cross-contamination if a DB
    // is ever reused or shared in a future migration scenario.
    public async Task<IEnumerable<ContentItem>> GetByTenantAndTypeAsync(
        Guid tenantId, string contentType, CancellationToken ct = default) =>
        await db.ContentItems
            .Where(c => c.TenantId == tenantId && c.ContentType == contentType)
            .OrderByDescending(c => c.PublishedAt)
            .AsNoTracking()
            .ToListAsync(ct);

    public Task<ContentItem?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct = default) =>
        db.ContentItems
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId, ct);
}
