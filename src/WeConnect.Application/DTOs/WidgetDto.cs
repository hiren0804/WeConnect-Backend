namespace WeConnect.Application.DTOs;

public record WidgetDetailDto(
    Guid Id,
    Guid PageId,
    string WidgetKey,
    string WidgetType,
    int Col,
    int Height,
    int DisplayOrder,
    bool IsVisible
);

public record CreateWidgetRequest(
    Guid PageId,
    string WidgetKey,
    string WidgetType,
    int Col,
    int Height,
    int DisplayOrder,
    bool IsVisible = true
);

public record UpdateWidgetRequest(
    Guid PageId,
    string WidgetKey,
    string WidgetType,
    int Col,
    int Height,
    int DisplayOrder,
    bool IsVisible
);

