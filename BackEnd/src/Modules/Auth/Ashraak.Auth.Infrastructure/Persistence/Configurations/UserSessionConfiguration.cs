using Ashraak.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Auth.Infrastructure.Persistence.Configurations;

internal sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("user_sessions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.TenantId).HasColumnName("tenant_id");
        builder.Property(x => x.CreatedOnUtc).HasColumnName("created_on_utc");
        builder.Property(x => x.LastUsedOnUtc).HasColumnName("last_used_on_utc");
        builder.Property(x => x.IpAddress).HasColumnName("ip_address").HasMaxLength(64);
        builder.Property(x => x.UserAgent).HasColumnName("user_agent").HasMaxLength(512);
        builder.Property(x => x.IsRevoked).HasColumnName("is_revoked");
        builder.Property(x => x.RevokedOnUtc).HasColumnName("revoked_on_utc");
        builder.HasIndex(x => new { x.UserId, x.TenantId, x.IsRevoked });
    }
}
