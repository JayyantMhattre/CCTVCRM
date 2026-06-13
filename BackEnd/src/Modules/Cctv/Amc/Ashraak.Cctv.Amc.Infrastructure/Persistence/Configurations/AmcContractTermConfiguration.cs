using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Amc.Infrastructure.Persistence.Configurations;

internal sealed class AmcContractTermConfiguration : IEntityTypeConfiguration<AmcContractTerm>
{
    public void Configure(EntityTypeBuilder<AmcContractTerm> builder)
    {
        builder.ToTable("amc_contract_terms");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, value => AmcContractTermId.From(value))
            .HasColumnName("id");

        builder.Property(t => t.ContractId)
            .HasConversion(id => id.Value, value => AmcContractId.From(value))
            .HasColumnName("amc_contract_id");

        builder.Property(t => t.TermNo).IsRequired().HasColumnName("term_no");
        builder.Property(t => t.PlanVersionId)
            .HasConversion(id => id.Value, value => AmcPlanVersionId.From(value))
            .HasColumnName("amc_plan_version_id");
        builder.Property(t => t.StartDate).IsRequired().HasColumnName("start_date");
        builder.Property(t => t.EndDate).IsRequired().HasColumnName("end_date");
        builder.Property(t => t.AgreedPrice).HasPrecision(18, 2).IsRequired().HasColumnName("agreed_price");
        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnName("status");
        builder.Property(t => t.Origin)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnName("origin");
        builder.Property(t => t.RenewalRequestedByCustomer).IsRequired().HasColumnName("renewal_requested_by_customer");
        builder.Property(t => t.RenewalRequestedAtUtc).HasColumnName("renewal_requested_at");
        builder.Property(t => t.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(t => t.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(t => t.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(t => t.UpdatedBy).HasColumnName("updated_by");
        builder.Property(t => t.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken();

        builder.HasIndex(t => new { t.ContractId, t.TermNo })
            .IsUnique()
            .HasDatabaseName("ux_amc_contract_terms_contract_id_term_no");
        builder.HasIndex(t => t.ContractId)
            .IsUnique()
            .HasFilter("status = 'Active'")
            .HasDatabaseName("ux_amc_contract_terms_contract_id_active");
        builder.HasIndex(t => t.EndDate).HasDatabaseName("ix_amc_contract_terms_end_date");

        builder.ToTable(t => t.HasCheckConstraint(
            "ck_amc_contract_terms_dates",
            "end_date > start_date"));
    }
}
