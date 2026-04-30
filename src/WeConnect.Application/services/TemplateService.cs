using WeConnect.Application.DTOs;
using WeConnect.Domain.Entities;
using WeConnect.Domain.Interfaces;

namespace WeConnect.Application.Services;

public class TemplateService
{
    public async Task<IEnumerable<TemplateDetailDto>> GetAllAsync(
        ITemplateRepository repo, CancellationToken ct = default)
    {
        var items = await repo.GetAllAsync(ct);
        return items.Select(MapToDto);
    }

    public async Task<TemplateDetailDto?> GetByIdAsync(
        ITemplateRepository repo, Guid id, CancellationToken ct = default)
    {
        var item = await repo.GetByIdAsync(id, ct);
        return item is null ? null : MapToDto(item);
    }

    public async Task<TemplateDetailDto?> GetByKeyAsync(
        ITemplateRepository repo, string key, CancellationToken ct = default)
    {
        var item = await repo.GetByKeyAsync(key, ct);
        return item is null ? null : MapToDto(item);
    }

    public async Task<TemplateDetailDto> CreateAsync(
        ITemplateRepository repo, CreateTemplateRequest request, CancellationToken ct = default)
    {
        var item = new Template
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Key = request.Key,
            Description = request.Description,
            ThumbnailUrl = request.ThumbnailUrl,
            IsActive = request.IsActive
        };

        var created = await repo.CreateAsync(item, ct);
        return MapToDto(created);
    }

    public async Task<TemplateDetailDto?> UpdateAsync(
        ITemplateRepository repo, Guid id, UpdateTemplateRequest request, CancellationToken ct = default)
    {
        var existing = await repo.GetByIdAsync(id, ct);
        if (existing is null) return null;

        existing.Name = request.Name;
        existing.Key = request.Key;
        existing.Description = request.Description;
        existing.ThumbnailUrl = request.ThumbnailUrl;
        existing.IsActive = request.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await repo.UpdateAsync(existing, ct);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(
        ITemplateRepository repo, Guid id, CancellationToken ct = default)
    {
        return await repo.DeleteAsync(id, ct);
    }

    private static TemplateDetailDto MapToDto(Template t) => new(
        t.Id,
        t.Name,
        t.Key,
        t.Description,
        t.ThumbnailUrl,
        t.IsActive,
        t.CreatedAt,
        t.UpdatedAt
    );
}

