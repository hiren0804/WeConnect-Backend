using Microsoft.EntityFrameworkCore;
using WeConnect.Domain.Entities;
using WeConnect.Domain.Interfaces;
using WeConnect.Infrastructure.Persistence;

namespace WeConnect.Infrastructure.Repositories;

public class WidgetRepository(MasterDbContext db) : IWidgetRepository
{
    public async Task<IEnumerable<TenantWidget>> GetAllAsync(
        CancellationToken ct = default) =>
        await db.Widgets
            .Include(w => w.Page)
            .OrderBy(w => w.DisplayOrder)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<IEnumerable<TenantWidget>> GetByPageIdAsync(
        Guid pageId, CancellationToken ct = default) =>
        await db.Widgets
            .Where(w => w.PageId == pageId)
            .OrderBy(w => w.DisplayOrder)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<TenantWidget?> GetByIdAsync(
        Guid id, CancellationToken ct = default) =>
        await db.Widgets
            .Include(w => w.Page)
            .FirstOrDefaultAsync(w => w.Id == id, ct);

    public async Task<TenantWidget> CreateAsync(
        TenantWidget widget, CancellationToken ct = default)
    {
        db.Widgets.Add(widget);
        await db.SaveChangesAsync(ct);
        return widget;
    }

    public async Task<TenantWidget> UpdateAsync(
        TenantWidget widget, CancellationToken ct = default)
    {
        db.Entry(widget).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
        return widget;
    }

    public async Task<bool> DeleteAsync(
        Guid id, CancellationToken ct = default)
    {
        var item = await db.Widgets
            .FirstOrDefaultAsync(w => w.Id == id, ct);
        if (item is null) return false;

        db.Widgets.Remove(item);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ExistsAsync(
        Guid id, CancellationToken ct = default) =>
        await db.Widgets
            .AnyAsync(w => w.Id == id, ct);
}
