namespace WeConnect.Domain.Entities;

public class TenantWidget
{
    public Guid Id { get; set; }
    public Guid PageId { get; set; }
    public string WidgetKey { get; set; } = string.Empty;  // "highlights","blogs"
    public string WidgetType { get; set; } = string.Empty;
    public int Col { get; set; } = 4;
    public int Height { get; set; } = 600;
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; } = true;

    public TenantPage Page { get; set; } = null!;
}
