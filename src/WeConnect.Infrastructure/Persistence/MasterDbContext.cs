namespace WeConnect.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using WeConnect.Domain.Entities;

// Always points to weconnect_master — fixed connection string
public class MasterDbContext(DbContextOptions<MasterDbContext> opts) : DbContext(opts)
{
    public DbSet<User>             Users            => Set<User>();
    public DbSet<Tenant>           Tenants          => Set<Tenant>();
    public DbSet<TenantModule>     Modules          => Set<TenantModule>();
    public DbSet<TenantPage>       Pages            => Set<TenantPage>();
    public DbSet<TenantWidget>     Widgets          => Set<TenantWidget>();
    public DbSet<TenantConnection> Connections      => Set<TenantConnection>();
    public DbSet<ProvisioningLog>  ProvisioningLogs => Set<ProvisioningLog>();
    public DbSet<Template>         Templates        => Set<Template>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Tenant>(e => {
            e.ToTable("tenants");
            e.HasIndex(t => t.Slug).IsUnique();
            e.HasIndex(t => t.Domain).IsUnique();
        });

        b.Entity<TenantConnection>(e => {
            e.ToTable("tenant_connections");
            e.HasIndex(c => c.TenantId).IsUnique();
            e.HasOne(c => c.Tenant)
             .WithOne()
             .HasForeignKey<TenantConnection>(c => c.TenantId);
        });

        b.Entity<TenantModule>(e => {
            e.ToTable("tenant_modules");
            e.HasOne(m => m.Tenant)
             .WithMany(t => t.Modules)
             .HasForeignKey(m => m.TenantId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<TenantPage>(e => {
            e.ToTable("tenant_pages");
            e.HasOne(p => p.Module)
             .WithMany(m => m.Pages)
             .HasForeignKey(p => p.ModuleId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<TenantWidget>(e => {
            e.ToTable("tenant_widgets");
            e.HasOne(w => w.Page)
             .WithMany(p => p.Widgets)
             .HasForeignKey(w => w.PageId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<ProvisioningLog>(e => {
            e.ToTable("provisioning_logs");
            e.HasOne(l => l.Tenant)
             .WithMany()
             .HasForeignKey(l => l.TenantId);
        });

        b.Entity<Template>(e => {
            e.ToTable("templates");
            e.HasIndex(t => t.Key).IsUnique();
            e.Property(t => t.IsActive).HasDefaultValue(true);
        });
    }
}


