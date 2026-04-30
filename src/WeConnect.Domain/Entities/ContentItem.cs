namespace WeConnect.Domain.Entities;

public class ContentItem
{
    public Guid      Id            { get; set; }
    public Guid      TenantId      { get; set; }   // kept for filtering — no FK in tenant DB
    public string    ContentType   { get; set; } = string.Empty;
    public string    Title         { get; set; } = string.Empty;
    public string?   Description   { get; set; }
    public string?   ImageUrl      { get; set; }
    public string?   AuthorName    { get; set; }
    public string?   AuthorImage   { get; set; }
    public int       Likes         { get; set; }
    public int       CommentsCount { get; set; }
    public int       Shares        { get; set; }
    public DateOnly? PublishedAt   { get; set; }
    public string?   ExtraJson     { get; set; }
    // No navigation property to Tenant — TenantDbContext has no Tenant table
}
