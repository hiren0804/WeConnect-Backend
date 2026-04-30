using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeConnect.Domain.Entities;

namespace WeConnect.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users");

        b.HasKey(u => u.Id);
        b.Property(u => u.Id).HasColumnName("id");

        b.Property(u => u.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        b.Property(u => u.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        b.Property(u => u.Email).HasColumnName("email").HasMaxLength(256).IsRequired();
        b.Property(u => u.AzureAdObjectId).HasColumnName("azure_ad_object_id").HasMaxLength(128);
        b.Property(u => u.ProfilePictureUrl).HasColumnName("profile_picture_url").HasMaxLength(512);
        b.Property(u => u.Role).HasColumnName("role").IsRequired();
        b.Property(u => u.Status).HasColumnName("status").IsRequired();
        b.Property(u => u.LastLoginAt).HasColumnName("last_login_at");
        b.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(u => u.UpdatedAt).HasColumnName("updated_at");
        b.Property(u => u.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false).IsRequired();
        b.Property(u => u.DeletedAt).HasColumnName("deleted_at");

        // Unique email + soft-delete global filter
        b.HasIndex(u => u.Email).IsUnique().HasFilter("\"is_deleted\" = false");
        b.HasIndex(u => u.AzureAdObjectId);
        b.HasQueryFilter(u => !u.IsDeleted);
    }
}