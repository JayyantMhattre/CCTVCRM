using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Enums;
using PlanAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Plan.AmcPlan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Amc.Infrastructure.Persistence.Configurations;

internal sealed class AmcPlanConfiguration : IEntityTypeConfiguration<PlanAggregate>
{
    public void Configure(EntityTypeBuilder<PlanAggregate> builder)
    {
        builder.ToTable("amc_plans");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => AmcPlanId.From(value))
            .HasColumnName("id");

        builder.Property(p => p.PlanCode).HasMaxLength(32).IsRequired().HasColumnName("plan_code");
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired().HasColumnName("name");
        builder.Property(p => p.Description).HasMaxLength(2000).HasColumnName("description");
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnName("status");
        builder.Property(p => p.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(p => p.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(p => p.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(p => p.UpdatedBy).HasColumnName("updated_by");
        builder.Property(p => p.IsDeleted).IsRequired().HasColumnName("is_deleted");
        builder.Property(p => p.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken();

        builder.HasIndex(p => p.PlanCode).IsUnique().HasDatabaseName("ux_amc_plans_plan_code");
        builder.HasIndex(p => p.Status).HasDatabaseName("ix_amc_plans_status");

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasMany(p => p.Versions)
            .WithOne()
            .HasForeignKey(v => v.PlanId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(p => p.Versions).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(p => p.DomainEvents);
    }
}
