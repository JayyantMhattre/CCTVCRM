using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Amc.Infrastructure.Persistence.Configurations;

internal sealed class AmcContractDocumentConfiguration : IEntityTypeConfiguration<AmcContractDocument>
{
    public void Configure(EntityTypeBuilder<AmcContractDocument> builder)
    {
        builder.ToTable("amc_contract_documents");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasConversion(id => id.Value, value => AmcContractDocumentId.From(value))
            .HasColumnName("id");

        builder.Property(d => d.ContractId)
            .HasConversion(id => id.Value, value => AmcContractId.From(value))
            .HasColumnName("amc_contract_id");

        builder.Property(d => d.TermId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? AmcContractTermId.From(value.Value) : null)
            .HasColumnName("amc_contract_term_id");

        builder.Property(d => d.FileId).IsRequired().HasColumnName("file_id");
        builder.Property(d => d.DocumentType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnName("document_type");
        builder.Property(d => d.Title).HasMaxLength(200).IsRequired().HasColumnName("title");
        builder.Property(d => d.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(d => d.CreatedBy).IsRequired().HasColumnName("created_by");

        builder.HasIndex(d => d.ContractId).HasDatabaseName("ix_amc_contract_documents_contract_id");
        builder.HasIndex(d => d.FileId).HasDatabaseName("ix_amc_contract_documents_file_id");
    }
}
