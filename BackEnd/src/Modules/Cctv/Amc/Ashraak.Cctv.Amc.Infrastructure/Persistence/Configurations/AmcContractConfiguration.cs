using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Enums;
using ContractAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Contract.AmcContract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Amc.Infrastructure.Persistence.Configurations;

internal sealed class AmcContractConfiguration : IEntityTypeConfiguration<ContractAggregate>
{
    public void Configure(EntityTypeBuilder<ContractAggregate> builder)
    {
        builder.ToTable("amc_contracts");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => AmcContractId.From(value))
            .HasColumnName("id");

        builder.Property(c => c.ContractNumber).HasMaxLength(32).IsRequired().HasColumnName("contract_number");
        builder.Property(c => c.SiteId).IsRequired().HasColumnName("site_id");
        builder.Property(c => c.CustomerId).IsRequired().HasColumnName("customer_id");
        builder.Property(c => c.SourceLeadId).HasColumnName("source_lead_id");
        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnName("status");
        builder.Property(c => c.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(c => c.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(c => c.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by");
        builder.Property(c => c.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken();

        builder.HasIndex(c => c.ContractNumber).IsUnique().HasDatabaseName("ux_amc_contracts_contract_number");
        builder.HasIndex(c => c.SiteId)
            .IsUnique()
            .HasFilter("status = 'Active'")
            .HasDatabaseName("ux_amc_contracts_site_id_active");
        builder.HasIndex(c => c.CustomerId).HasDatabaseName("ix_amc_contracts_customer_id");
        builder.HasIndex(c => c.Status).HasDatabaseName("ix_amc_contracts_status");

        builder.HasMany(c => c.Terms)
            .WithOne()
            .HasForeignKey(t => t.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(c => c.Terms).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(c => c.Documents)
            .WithOne()
            .HasForeignKey(d => d.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(c => c.Documents).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(c => c.DomainEvents);
    }
}
