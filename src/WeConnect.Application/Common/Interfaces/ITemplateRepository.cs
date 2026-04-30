using WeConnect.Domain.Entities;

namespace WeConnect.Domain.Interfaces;

public interface ITemplateRepository
{
    Task<IEnumerable<Template>> GetAllAsync(CancellationToken ct = default);
    Task<Template?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Template?> GetByKeyAsync(string key, CancellationToken ct = default);
    Task<Template> CreateAsync(Template template, CancellationToken ct = default);
    Task<Template> UpdateAsync(Template template, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}

