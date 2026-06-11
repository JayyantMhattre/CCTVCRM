using Ashraak.Auth.Domain.Aggregates.Invitation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Auth.Infrastructure.Persistence.Configurations;

internal sealed class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.ToTable("invitations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => InvitationId.From(value));
        builder.Property(x => x.TenantId).HasColumnName("tenant_id");
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(320);
        builder.Property(x => x.Role).HasColumnName("role").HasMaxLength(100);
        builder.Property(x => x.TokenHash).HasColumnName("token_hash").HasMaxLength(128);
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.ExpiresOnUtc).HasColumnName("expires_on_utc");
        builder.Property(x => x.InvitedByUserId).HasColumnName("invited_by_user_id");
        builder.Property(x => x.AcceptedByUserId).HasColumnName("accepted_by_user_id");
        builder.Property(x => x.CreatedOnUtc).HasColumnName("created_on_utc");
        builder.Property(x => x.UpdatedOnUtc).HasColumnName("updated_on_utc");
        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.Email, x.Status });
        builder.Ignore(x => x.DomainEvents);
    }
}
