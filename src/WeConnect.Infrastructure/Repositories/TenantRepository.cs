    namespace WeConnect.Infrastructure.Repositories;

    using Microsoft.EntityFrameworkCore;
    using WeConnect.Application.Common.Interfaces;
    using WeConnect.Domain.Entities;
    using WeConnect.Infrastructure.Persistence;
    using System.Linq.Expressions;

    public class TenantRepository(MasterDbContext db) : ITenantRepository
    {
        public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct) =>
            db.Tenants
                .Include(t => t.Modules.Where(m => m.IsEnabled))
                    .ThenInclude(m => m.Pages)
                        .ThenInclude(p => p.Widgets.Where(w => w.IsVisible))
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Slug == slug && t.IsActive, ct);

        public async Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken ct) =>
            await db.Tenants.AsNoTracking().ToListAsync(ct);

        void IRepository<Tenant>.Update(Tenant tenant) => db.Tenants.Update(tenant);

        void IRepository<Tenant>.Delete(Tenant tenant) => db.Tenants.Remove(tenant);

        Task<int> IRepository<Tenant>.SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
    }
