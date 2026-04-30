namespace WeConnect.Application.DTOs;

public record ContentItemDto(
    Guid    Id,
    string  Title,
    string? Description,
    string? Image,
    string? AuthorName,
    string? AuthorImage,
    int     Likes,
    int     Comments,
    int     Shares,
    bool    IsLiked,
    string? Date,
    string? RComments
);