namespace WeConnect.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using WeConnect.Domain.Entities;
public class TenantDbContext(DbContextOptions<TenantDbContext> opts) : DbContext(opts)
{
    public DbSet<ContentItem> ContentItems => Set<ContentItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<ContentItem>(e => {
            e.ToTable("content_items");
            e.Property(c => c.ExtraJson).HasColumnType("jsonb");
            // TenantId is just a plain column — no FK constraint (no Tenant table here)
            e.Property(c => c.TenantId).IsRequired();
        });
    }
}
