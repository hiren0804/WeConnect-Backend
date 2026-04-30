using WeConnect.Application.DTOs;
using WeConnect.Domain.Entities;
using WeConnect.Domain.Interfaces;

namespace WeConnect.Application.Services;

public class ListingService
{
    public async Task<IEnumerable<ListingDto>> GetAllAsync(
        IListingRepository repo, Guid tenantId, CancellationToken ct = default)
    {
        var items = await repo.GetAllAsync(tenantId, ct);
        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<ListingDto>> GetByTypeAsync(
        IListingRepository repo, Guid tenantId, string contentType, CancellationToken ct = default)
    {
        var items = await repo.GetByTypeAsync(tenantId, contentType, ct);
        return items.Select(MapToDto);
    }

    public async Task<ListingDto?> GetByIdAsync(
        IListingRepository repo, Guid id, Guid tenantId, CancellationToken ct = default)
    {
        var item = await repo.GetByIdAsync(id, tenantId, ct);
        return item is null ? null : MapToDto(item);
    }

    public async Task<ListingDto> CreateAsync(
        IListingRepository repo, Guid tenantId, CreateListingRequest request, CancellationToken ct = default)
    {
        var item = new ContentItem
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ContentType = request.ContentType,
            Title = request.Title,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            AuthorName = request.AuthorName,
            AuthorImage = request.AuthorImage,
            Likes = request.Likes,
            CommentsCount = request.CommentsCount,
            Shares = request.Shares,
            PublishedAt = request.PublishedAt,
            ExtraJson = request.ExtraJson
        };

        var created = await repo.CreateAsync(item, ct);
        return MapToDto(created);
    }

    public async Task<ListingDto?> UpdateAsync(
        IListingRepository repo, Guid id, Guid tenantId, UpdateListingRequest request, CancellationToken ct = default)
    {
        var existing = await repo.GetByIdAsync(id, tenantId, ct);
        if (existing is null) return null;

        existing.ContentType = request.ContentType;
        existing.Title = request.Title;
        existing.Description = request.Description;
        existing.ImageUrl = request.ImageUrl;
        existing.AuthorName = request.AuthorName;
        existing.AuthorImage = request.AuthorImage;
        existing.Likes = request.Likes;
        existing.CommentsCount = request.CommentsCount;
        existing.Shares = request.Shares;
        existing.PublishedAt = request.PublishedAt;
        existing.ExtraJson = request.ExtraJson;

        var updated = await repo.UpdateAsync(existing, ct);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(
        IListingRepository repo, Guid id, Guid tenantId, CancellationToken ct = default)
    {
        return await repo.DeleteAsync(id, tenantId, ct);
    }

    private static ListingDto MapToDto(ContentItem c) => new(
        c.Id,
        c.ContentType,
        c.Title,
        c.Description,
        c.ImageUrl,
        c.AuthorName,
        c.AuthorImage,
        c.Likes,
        c.CommentsCount,
        c.Shares,
        c.PublishedAt,
        c.ExtraJson,
        DateTime.UtcNow // Not tracked in entity; use current time
    );
}

