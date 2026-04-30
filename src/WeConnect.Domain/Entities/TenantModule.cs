namespace WeConnect.Domain.Entities;

public class TenantModule
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ModuleKey { get; set; } = string.Empty;  // "dashboard","blogs"
    public bool IsEnabled { get; set; } = true;
    public int DisplayOrder { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public ICollection<TenantPage> Pages { get; set; } = [];
}