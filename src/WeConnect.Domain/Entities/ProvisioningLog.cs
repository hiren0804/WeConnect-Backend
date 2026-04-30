namespace WeConnect.Domain.Entities;

public class ProvisioningLog
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Step { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
}