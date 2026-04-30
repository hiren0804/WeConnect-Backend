namespace WeConnect.Domain.Common;

public abstract class BaseEntity
{
    public Guid     Id        { get; private set; }  = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; }  = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public bool     IsDeleted  { get; private set; }  = false;
    public DateTime? DeletedAt { get; private set; }

    public void SoftDelete()
    {
        IsDeleted  = true;
        DeletedAt  = DateTime.UtcNow;
        UpdatedAt  = DateTime.UtcNow;
    }

    public void MarkUpdated() => UpdatedAt = DateTime.UtcNow;
}