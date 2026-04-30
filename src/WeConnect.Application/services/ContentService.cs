namespace WeConnect.Application.Services;

using System.Text.Json;
using WeConnect.Application.DTOs;
using WeConnect.Domain.Entities;
using WeConnect.Domain.Interfaces;

public class ContentService
{
    // Stateless service — repository is passed per-request by the controller
    // so that the correct per-tenant DbContext is used.

    public async Task<IEnumerable<ContentItemDto>> GetItemsAsync(
        IContentRepository repo, Guid tenantId, string type, CancellationToken ct = default)
    {
        var items = await repo.GetByTenantAndTypeAsync(tenantId, type, ct);
        return items.Select(MapToDto);
    }

    public async Task<ContentItemDto?> GetItemAsync(
        IContentRepository repo, Guid id, Guid tenantId, CancellationToken ct = default)
    {
        var item = await repo.GetByIdAsync(id, tenantId, ct);
        return item is null ? null : MapToDto(item);
    }

    private static ContentItemDto MapToDto(ContentItem c)
    {
        string? rcomments = null;

        if (c.ExtraJson is not null)
        {
            try {
                var doc = JsonDocument.Parse(c.ExtraJson);
                if (doc.RootElement.TryGetProperty("rcomments", out var el))
                    rcomments = el.GetString();
            }
            catch (JsonException) { /* malformed json — skip gracefully */ }
        }

        return new ContentItemDto(
            Id:          c.Id,
            Title:       c.Title,
            Description: c.Description,
            Image:       c.ImageUrl,
            AuthorName:  c.AuthorName,
            AuthorImage: c.AuthorImage,
            Likes:       c.Likes,
            Comments:    c.CommentsCount,
            Shares:      c.Shares,
            IsLiked:     false,   // stateless for MVP
            Date:        c.PublishedAt?.ToString("MMMM dd, yyyy"),
            RComments:   rcomments
        );
    }
}

