namespace WeConnect.Domain.Entities;

public class TenantConnection
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public Tenant Tenant { get; set; } = null!;

    // Build connection string at runtime — never store the full string
    public string BuildConnectionString() =>
        $"Host={Host};Port={Port};Database={Database};" +
        $"Username={Username};Password={Password};Pooling=true;MaxPoolSize=10";
}