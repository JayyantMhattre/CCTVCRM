using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Amc.Infrastructure.Persistence.Configurations;

internal sealed class AmcPlanVersionConfiguration : IEntityTypeConfiguration<AmcPlanVersion>
{
    public void Configure(EntityTypeBuilder<AmcPlanVersion> builder)
    {
        builder.ToTable("amc_plan_versions");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasConversion(id => id.Value, value => AmcPlanVersionId.From(value))
            .HasColumnName("id");

        builder.Property(v => v.PlanId)
            .HasConversion(id => id.Value, value => AmcPlanId.From(value))
            .HasColumnName("amc_plan_id");

        builder.Property(v => v.VersionNo).IsRequired().HasColumnName("version_no");
        builder.Property(v => v.Price).HasPrecision(18, 2).IsRequired().HasColumnName("price");
        builder.Property(v => v.VisitFrequencyPerYear).IsRequired().HasColumnName("visit_frequency_per_year");
        builder.Property(v => v.IncludedServicesJson).IsRequired().HasColumnName("included_services");
        builder.Property(v => v.SlaTerms).HasMaxLength(4000).IsRequired().HasColumnName("sla_terms");
        builder.Property(v => v.EffectiveFrom).IsRequired().HasColumnName("effective_from");
        builder.Property(v => v.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnName("status");
        builder.Property(v => v.IsReferenced).IsRequired().HasColumnName("is_referenced");
        builder.Property(v => v.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(v => v.CreatedBy).IsRequired().HasColumnName("created_by");

        builder.HasIndex(v => new { v.PlanId, v.VersionNo })
            .IsUnique()
            .HasDatabaseName("ux_amc_plan_versions_plan_id_version_no");
    }
}
