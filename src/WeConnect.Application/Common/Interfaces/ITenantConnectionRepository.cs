namespace WeConnect.Domain.Interfaces;

using WeConnect.Domain.Entities;

public interface ITenantConnectionRepository
{
    Task<TenantConnection?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task AddAsync(TenantConnection conn, CancellationToken ct = default);
}