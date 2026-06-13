using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.Cctv.Invoice.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Invoice.Infrastructure.Persistence.Configurations;

internal sealed class InvoiceStatusHistoryConfiguration : IEntityTypeConfiguration<InvoiceStatusHistory>
{
    public void Configure(EntityTypeBuilder<InvoiceStatusHistory> builder)
    {
        builder.ToTable("invoice_status_histories");

        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id)
            .HasConversion(id => id.Value, value => InvoiceStatusHistoryId.From(value))
            .HasColumnName("id");

        builder.Property(h => h.InvoiceId)
            .HasConversion(id => id.Value, value => InvoiceId.From(value))
            .IsRequired()
            .HasColumnName("invoice_id");

        builder.Property(h => h.FromStatus).HasConversion<string>().HasMaxLength(32).HasColumnName("from_status");
        builder.Property(h => h.ToStatus).HasConversion<string>().HasMaxLength(32).IsRequired().HasColumnName("to_status");
        builder.Property(h => h.ChangedAtUtc).IsRequired().HasColumnName("changed_at");
        builder.Property(h => h.ChangedBy).IsRequired().HasColumnName("changed_by");
        builder.Property(h => h.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(h => h.CreatedBy).IsRequired().HasColumnName("created_by");
    }
}
