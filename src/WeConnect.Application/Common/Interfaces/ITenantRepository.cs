namespace WeConnect.Application.Common.Interfaces;

using WeConnect.Domain.Entities;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken ct = default);
}
