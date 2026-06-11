using Ashraak.Users.Domain.Aggregates.UserProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Users.Infrastructure.Persistence.Configurations;

internal sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("profiles");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .HasColumnName("id");

        builder.Property(u => u.TenantId).HasColumnName("tenant_id");
        builder.Property(u => u.Email).HasMaxLength(254).HasColumnName("email");
        builder.Property(u => u.DisplayName).HasMaxLength(100).HasColumnName("display_name");
        builder.Property(u => u.AvatarUrl).HasMaxLength(2048).HasColumnName("avatar_url");
        builder.Property(u => u.Status).HasConversion<string>().HasColumnName("status");
        builder.Property(u => u.CreatedOnUtc).HasColumnName("created_on_utc");
        builder.Property(u => u.UpdatedOnUtc).HasColumnName("updated_on_utc");

        // Preferences are profile/business data and remain within the Users module schema.
        builder.OwnsOne(u => u.Preferences, p =>
        {
            p.Property(x => x.Theme).HasColumnName("pref_theme").HasMaxLength(20);
            p.Property(x => x.Locale).HasColumnName("pref_locale").HasMaxLength(20);
            p.Property(x => x.Timezone).HasColumnName("pref_timezone").HasMaxLength(50);
            p.Property(x => x.EmailNotificationsEnabled).HasColumnName("pref_email_notifications_enabled");
        });

        builder.HasIndex(u => new { u.Email, u.TenantId }).IsUnique();
        builder.HasIndex(u => u.TenantId);

        builder.HasMany(u => u.Memberships)
            .WithOne()
            .HasForeignKey("user_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(u => u.DomainEvents);
    }
}
