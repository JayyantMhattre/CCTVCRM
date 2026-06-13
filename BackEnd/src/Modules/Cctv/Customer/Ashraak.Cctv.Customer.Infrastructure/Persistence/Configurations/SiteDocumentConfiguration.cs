using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using Ashraak.Cctv.Customer.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Customer.Infrastructure.Persistence.Configurations;

internal sealed class SiteDocumentConfiguration : IEntityTypeConfiguration<SiteDocument>
{
    public void Configure(EntityTypeBuilder<SiteDocument> builder)
    {
        builder.ToTable("site_documents");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasConversion(id => id.Value, value => SiteDocumentId.From(value))
            .HasColumnName("id");

        builder.Property(d => d.SiteId)
            .HasConversion(id => id.Value, value => SiteId.From(value))
            .HasColumnName("site_id");

        builder.Property(d => d.FileId).IsRequired().HasColumnName("file_id");
        builder.Property(d => d.DocumentType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnName("document_type");
        builder.Property(d => d.Title).HasMaxLength(200).IsRequired().HasColumnName("title");
        builder.Property(d => d.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(d => d.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(d => d.IsDeleted).IsRequired().HasColumnName("is_deleted");

        builder.HasIndex(d => d.SiteId).HasDatabaseName("ix_site_documents_site_id");
    }
}
