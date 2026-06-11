using Ashraak.Auth.Domain.Aggregates.AuthUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Auth.Infrastructure.Persistence.Configurations;

internal sealed class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(id => id.Value, value => AuthUserId.From(value))
            .HasColumnName("id");

        builder.Property(u => u.TenantId).HasColumnName("tenant_id");
        builder.Property(u => u.Email).HasMaxLength(254).HasColumnName("email");
        builder.Property(u => u.PasswordHash).HasColumnName("password_hash");
        builder.Property(u => u.EmailVerified).HasColumnName("email_verified");
        builder.Property(u => u.MfaEnabled).HasColumnName("mfa_enabled");
        builder.Property(u => u.MfaSecret).HasColumnName("mfa_secret");
        builder.Property(u => u.IsActive).HasColumnName("is_active");
        builder.Property(u => u.FailedLoginAttempts).HasColumnName("failed_login_attempts");
        builder.Property(u => u.LockedUntilUtc).HasColumnName("locked_until_utc");
        builder.Property(u => u.CreatedOnUtc).HasColumnName("created_on_utc");
        builder.Property(u => u.UpdatedOnUtc).HasColumnName("updated_on_utc");

        builder.HasIndex(u => new { u.Email, u.TenantId }).IsUnique();

        builder.Ignore(u => u.DomainEvents);
    }
}
