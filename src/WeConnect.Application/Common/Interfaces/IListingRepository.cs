using WeConnect.Domain.Entities;

namespace WeConnect.Domain.Interfaces;

public interface IListingRepository
{
    Task<IEnumerable<ContentItem>> GetAllAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<ContentItem>> GetByTypeAsync(Guid tenantId, string contentType, CancellationToken ct = default);
    Task<ContentItem?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct = default);
    Task<ContentItem> CreateAsync(ContentItem item, CancellationToken ct = default);
    Task<ContentItem> UpdateAsync(ContentItem item, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, Guid tenantId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, Guid tenantId, CancellationToken ct = default);
}

