using WeConnect.Domain.Common;

namespace WeConnect.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name       { get; set; } = string.Empty;
    public string Slug       { get; set; } = string.Empty;
    public string Domain     { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public string Status     { get; set; } = string.Empty;
    public bool   IsActive   { get; set; } = true;

    public ICollection<TenantModule> Modules { get; set; } = [];
}
