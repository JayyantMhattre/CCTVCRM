using Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer;
using Ashraak.Cctv.Engineer.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EngineerAggregate = Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer.Engineer;

namespace Ashraak.Cctv.Engineer.Infrastructure.Persistence.Configurations;

internal sealed class EngineerConfiguration : IEntityTypeConfiguration<EngineerAggregate>
{
    public void Configure(EntityTypeBuilder<EngineerAggregate> builder)
    {
        builder.ToTable("engineers");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasConversion(id => id.Value, value => EngineerId.From(value))
            .HasColumnName("id");
        builder.Property(e => e.EngineerNumber).HasMaxLength(32).IsRequired().HasColumnName("engineer_number");
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired().HasColumnName("name");
        builder.Property(e => e.Phone).HasMaxLength(32).IsRequired().HasColumnName("phone");
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(32).IsRequired().HasColumnName("status");
        builder.Property(e => e.PlatformUserId).HasColumnName("platform_user_id");
        builder.Property(e => e.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(e => e.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(e => e.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        builder.Property(e => e.RowVersion).HasColumnName("row_version").IsConcurrencyToken();
        builder.HasIndex(e => e.EngineerNumber).IsUnique().HasDatabaseName("ux_engineers_engineer_number");
        builder.HasIndex(e => e.PlatformUserId).HasDatabaseName("ix_engineers_platform_user_id");
        builder.Ignore(e => e.IsActive);
        builder.Ignore(e => e.DomainEvents);
    }
}
