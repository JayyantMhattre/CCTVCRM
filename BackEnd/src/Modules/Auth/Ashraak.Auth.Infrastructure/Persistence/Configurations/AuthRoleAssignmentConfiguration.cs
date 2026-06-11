using Ashraak.Auth.Infrastructure.Persistence.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Auth.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core mapping for the RBAC user-role assignment table.
/// </summary>
internal sealed class AuthRoleAssignmentConfiguration : IEntityTypeConfiguration<AuthRoleAssignment>
{
    public void Configure(EntityTypeBuilder<AuthRoleAssignment> builder)
    {
        builder.ToTable("role_assignments");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TenantId).HasColumnName("tenant_id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.RoleName).HasMaxLength(128).HasColumnName("role_name");
        builder.Property(x => x.CreatedOnUtc).HasColumnName("created_on_utc");

        builder.HasIndex(x => new { x.TenantId, x.UserId, x.RoleName }).IsUnique();
    }
}
