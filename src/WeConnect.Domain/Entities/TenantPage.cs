namespace WeConnect.Domain.Entities;

public class TenantPage
{
    public Guid Id { get; set; }
    public Guid ModuleId { get; set; }
    public string PageKey { get; set; } = string.Empty;  // "home","detail"
    public string PageType { get; set; } = string.Empty;  // "widget","listing"
    public string Route { get; set; } = string.Empty;
    public string? CardComponent { get; set; }   // "photouicard"

    public TenantModule Module { get; set; } = null!;
    public ICollection<TenantWidget> Widgets { get; set; } = [];
}