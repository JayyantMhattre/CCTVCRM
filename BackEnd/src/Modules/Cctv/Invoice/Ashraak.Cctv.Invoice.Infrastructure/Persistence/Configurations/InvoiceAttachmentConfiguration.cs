using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.Cctv.Invoice.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Invoice.Infrastructure.Persistence.Configurations;

internal sealed class InvoiceAttachmentConfiguration : IEntityTypeConfiguration<InvoiceAttachment>
{
    public void Configure(EntityTypeBuilder<InvoiceAttachment> builder)
    {
        builder.ToTable("invoice_attachments", tb =>
        {
            tb.HasCheckConstraint(
                "ck_invoice_attachments_type",
                "attachment_type IN ('InvoicePdf','Supporting')");
        });

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasConversion(id => id.Value, value => InvoiceAttachmentId.From(value))
            .HasColumnName("id");

        builder.Property(a => a.InvoiceId)
            .HasConversion(id => id.Value, value => InvoiceId.From(value))
            .IsRequired()
            .HasColumnName("invoice_id");

        builder.Property(a => a.FileId).IsRequired().HasColumnName("file_id");
        builder.Property(a => a.AttachmentType).HasConversion<string>().HasMaxLength(32).IsRequired().HasColumnName("attachment_type");
        builder.Property(a => a.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(a => a.CreatedBy).IsRequired().HasColumnName("created_by");
    }
}
