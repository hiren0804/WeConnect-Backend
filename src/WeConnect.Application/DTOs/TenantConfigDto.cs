namespace WeConnect.Application.DTOs;

public record TenantConfigDto(
    ClientDto              Client,
    TemplateDto            Template,
    IEnumerable<ModuleDto> Modules
);

public record ClientDto(string Name, string Slug, string Domain);

public record TemplateDto(string Id);

public record ModuleDto(
    string               Id,
    bool                 Enabled,
    IEnumerable<PageDto> Pages
);

public record PageDto(
    string                 Id,
    string                 Type,
    string                 Route,
    string?                Card,
    IEnumerable<WidgetDto> Widgets
);

public record WidgetDto(
    string Id,
    string Type,
    int    Col,
    int    Height,
    int    Order,
    bool   Visible
);