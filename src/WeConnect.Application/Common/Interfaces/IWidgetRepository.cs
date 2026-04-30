using WeConnect.Domain.Entities;

namespace WeConnect.Domain.Interfaces;

public interface IWidgetRepository
{
    Task<IEnumerable<TenantWidget>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<TenantWidget>> GetByPageIdAsync(Guid pageId, CancellationToken ct = default);
    Task<TenantWidget?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TenantWidget> CreateAsync(TenantWidget widget, CancellationToken ct = default);
    Task<TenantWidget> UpdateAsync(TenantWidget widget, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}

