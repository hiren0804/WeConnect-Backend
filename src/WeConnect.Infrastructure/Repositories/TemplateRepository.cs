using Microsoft.EntityFrameworkCore;
using WeConnect.Domain.Entities;
using WeConnect.Domain.Interfaces;
using WeConnect.Infrastructure.Persistence;

namespace WeConnect.Infrastructure.Repositories;

public class TemplateRepository(MasterDbContext db) : ITemplateRepository
{
    public async Task<IEnumerable<Template>> GetAllAsync(
        CancellationToken ct = default) =>
        await db.Templates
            .OrderBy(t => t.Name)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<Template?> GetByIdAsync(
        Guid id, CancellationToken ct = default) =>
        await db.Templates
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<Template?> GetByKeyAsync(
        string key, CancellationToken ct = default) =>
        await db.Templates
            .FirstOrDefaultAsync(t => t.Key == key, ct);

    public async Task<Template> CreateAsync(
        Template template, CancellationToken ct = default)
    {
        db.Templates.Add(template);
        await db.SaveChangesAsync(ct);
        return template;
    }

    public async Task<Template> UpdateAsync(
        Template template, CancellationToken ct = default)
    {
        db.Entry(template).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
        return template;
    }

    public async Task<bool> DeleteAsync(
        Guid id, CancellationToken ct = default)
    {
        var item = await db.Templates
            .FirstOrDefaultAsync(t => t.Id == id, ct);
        if (item is null) return false;

        db.Templates.Remove(item);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ExistsAsync(
        Guid id, CancellationToken ct = default) =>
        await db.Templates
            .AnyAsync(t => t.Id == id, ct);
}

