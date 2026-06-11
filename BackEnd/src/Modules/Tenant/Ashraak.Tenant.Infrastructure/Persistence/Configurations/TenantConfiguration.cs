using Ashraak.Tenant.Domain.Aggregates.Tenant;
using Ashraak.Tenant.Domain.Aggregates.Tenant.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Tenant.Infrastructure.Persistence.Configurations;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<Domain.Aggregates.Tenant.Tenant>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Tenant.Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, value => TenantId.From(value))
            .HasColumnName("id");

        builder.Property(t => t.Name)
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(t => t.Slug)
            .HasMaxLength(50)
            .HasColumnName("slug");

        builder.HasIndex(t => t.Slug).IsUnique();

        builder.Property(t => t.Plan)
            .HasColumnName("plan")
            .HasConversion<string>();

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasConversion<string>();

        builder.Property(t => t.CustomDomain)
            .HasMaxLength(253)
            .HasColumnName("custom_domain");

        builder.OwnsOne(t => t.Settings, s =>
        {
            s.Property(x => x.Locale).HasColumnName("settings_locale").HasMaxLength(20);
            s.Property(x => x.Timezone).HasColumnName("settings_timezone").HasMaxLength(50);
            s.Property(x => x.PasswordMinLength).HasColumnName("settings_password_min_length");
            s.Property(x => x.RequireMfa).HasColumnName("settings_require_mfa");
            s.Property(x => x.SessionTimeoutMinutes).HasColumnName("settings_session_timeout_minutes");
        });

        builder.OwnsOne(t => t.Subscription, s =>
        {
            s.Property(x => x.Plan).HasColumnName("subscription_plan").HasConversion<string>();
            s.Property(x => x.SeatLimit).HasColumnName("subscription_seat_limit");
            s.Property(x => x.StorageLimitGb).HasColumnName("subscription_storage_limit_gb");
            s.Property(x => x.ExpiresOnUtc).HasColumnName("subscription_expires_on_utc");
        });

        builder.Property(t => t.CreatedOnUtc).HasColumnName("created_on_utc");
        builder.Property(t => t.UpdatedOnUtc).HasColumnName("updated_on_utc");

        builder.Ignore(t => t.DomainEvents);
    }
}
