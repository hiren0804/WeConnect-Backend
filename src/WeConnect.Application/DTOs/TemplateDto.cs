namespace WeConnect.Application.DTOs;

public record TemplateDetailDto(
    Guid Id,
    string Name,
    string Key,
    string? Description,
    string? ThumbnailUrl,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateTemplateRequest(
    string Name,
    string Key,
    string? Description,
    string? ThumbnailUrl,
    bool IsActive = true
);

public record UpdateTemplateRequest(
    string Name,
    string Key,
    string? Description,
    string? ThumbnailUrl,
    bool IsActive
);

