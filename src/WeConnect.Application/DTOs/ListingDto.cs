namespace WeConnect.Application.DTOs;

public record ListingDto(
    Guid Id,
    string ContentType,
    string Title,
    string? Description,
    string? ImageUrl,
    string? AuthorName,
    string? AuthorImage,
    int Likes,
    int CommentsCount,
    int Shares,
    DateOnly? PublishedAt,
    string? ExtraJson,
    DateTime CreatedAt
);

public record CreateListingRequest(
    string ContentType,
    string Title,
    string? Description,
    string? ImageUrl,
    string? AuthorName,
    string? AuthorImage,
    int Likes,
    int CommentsCount,
    int Shares,
    DateOnly? PublishedAt,
    string? ExtraJson
);

public record UpdateListingRequest(
    string ContentType,
    string Title,
    string? Description,
    string? ImageUrl,
    string? AuthorName,
    string? AuthorImage,
    int Likes,
    int CommentsCount,
    int Shares,
    DateOnly? PublishedAt,
    string? ExtraJson
);

