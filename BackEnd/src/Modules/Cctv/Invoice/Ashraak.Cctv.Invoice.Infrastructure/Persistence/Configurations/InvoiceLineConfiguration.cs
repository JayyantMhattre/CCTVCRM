using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Invoice.Infrastructure.Persistence.Configurations;

internal sealed class InvoiceLineConfiguration : IEntityTypeConfiguration<InvoiceLine>
{
    public void Configure(EntityTypeBuilder<InvoiceLine> builder)
    {
        builder.ToTable("invoice_lines");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .HasConversion(id => id.Value, value => InvoiceLineId.From(value))
            .HasColumnName("id");

        builder.Property(l => l.InvoiceId)
            .HasConversion(id => id.Value, value => InvoiceId.From(value))
            .IsRequired()
            .HasColumnName("invoice_id");

        builder.Property(l => l.LineNo).IsRequired().HasColumnName("line_no");
        builder.Property(l => l.Description).HasMaxLength(500).IsRequired().HasColumnName("description");
        builder.Property(l => l.Quantity).HasPrecision(9, 2).IsRequired().HasColumnName("quantity");
        builder.Property(l => l.UnitPrice).HasPrecision(18, 2).IsRequired().HasColumnName("unit_price");
        builder.Property(l => l.LineTotal).HasPrecision(18, 2).IsRequired().HasColumnName("line_total");
        builder.Property(l => l.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(l => l.CreatedBy).IsRequired().HasColumnName("created_by");

        builder.HasIndex(l => new { l.InvoiceId, l.LineNo })
            .IsUnique()
            .HasDatabaseName("ux_invoice_lines_invoice_id_line_no");
    }
}
