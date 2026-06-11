using Ashraak.Auth.Infrastructure.Persistence.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Auth.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core mapping for the permission grant table used by RBAC and ABAC.
/// </summary>
internal sealed class AuthPermissionGrantConfiguration : IEntityTypeConfiguration<AuthPermissionGrant>
{
    public void Configure(EntityTypeBuilder<AuthPermissionGrant> builder)
    {
        builder.ToTable("permission_grants");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TenantId).HasColumnName("tenant_id");
        builder.Property(x => x.RoleName).HasMaxLength(128).HasColumnName("role_name");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.Permission).HasMaxLength(256).HasColumnName("permission");
        builder.Property(x => x.ConditionExpression).HasMaxLength(256).HasColumnName("condition_expression");
        builder.Property(x => x.CreatedOnUtc).HasColumnName("created_on_utc");

        builder.HasIndex(x => new { x.TenantId, x.RoleName, x.Permission });
        builder.HasIndex(x => new { x.TenantId, x.UserId, x.Permission });
    }
}
