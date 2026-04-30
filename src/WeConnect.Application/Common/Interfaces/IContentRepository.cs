// WeConnect.Domain/Interfaces/IContentRepository.cs
namespace WeConnect.Domain.Interfaces;

using WeConnect.Domain.Entities;

public interface IContentRepository
{
    Task<IEnumerable<ContentItem>> GetByTenantAndTypeAsync(
        Guid tenantId, string contentType, CancellationToken ct = default);
    
    Task<ContentItem?> GetByIdAsync(
        Guid id, Guid tenantId, CancellationToken ct = default);
}