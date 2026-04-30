namespace WeConnect.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WeConnect.Domain.Entities;
using WeConnect.Domain.Interfaces;
using WeConnect.Infrastructure.Persistence;

public class TenantConnectionRepository(MasterDbContext db) : ITenantConnectionRepository
{
    public async Task<TenantConnection?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var tenant = await db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug == slug, ct);

        if (tenant is null) return null;

        return await db.Connections
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.TenantId == tenant.Id, ct);
    }

    public async Task AddAsync(TenantConnection conn, CancellationToken ct = default)
    {
        db.Connections.Add(conn);
        await db.SaveChangesAsync(ct);
    }
}

