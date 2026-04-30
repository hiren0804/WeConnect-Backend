using WeConnect.Application.DTOs;
using WeConnect.Domain.Entities;

namespace WeConnect.Application.Common.Interfaces;

public interface ITenantService
{
    Task<TenantConfigDto?> GetConfigAsync(string slug, CancellationToken ct = default);
    Task<Tenant?> GetTenantAsync(string slug, CancellationToken ct = default);
    Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken ct = default);
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<Tenant> UpdateAsync(Tenant tenant, CancellationToken ct = default);
    Task<bool> DeleteAsync(string slug, CancellationToken ct = default);
}
