using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.Cctv.Invoice.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvoiceAggregate = Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice.Invoice;

namespace Ashraak.Cctv.Invoice.Infrastructure.Persistence.Configurations;

internal sealed class InvoiceConfiguration : IEntityTypeConfiguration<InvoiceAggregate>
{
    public void Configure(EntityTypeBuilder<InvoiceAggregate> builder)
    {
        builder.ToTable("invoices", tb =>
        {
            tb.HasCheckConstraint(
                "ck_invoices_status",
                "status IN ('Draft','Generated','Sent','Paid','Cancelled')");
            tb.HasCheckConstraint(
                "ck_invoices_invoice_type",
                "invoice_type IN ('AmcRenewal','NewAmc','EmergencyService','SpareReplacement','AdditionalCharges','Other')");
            tb.HasCheckConstraint(
                "ck_invoices_amc_term_required",
                "invoice_type NOT IN ('AmcRenewal','NewAmc') OR amc_contract_term_id IS NOT NULL");
            tb.HasCheckConstraint(
                "ck_invoices_amounts",
                "subtotal_amount >= 0 AND tax_amount >= 0 AND total_amount >= 0 AND total_amount = subtotal_amount + tax_amount");
        });

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasConversion(id => id.Value, value => InvoiceId.From(value))
            .HasColumnName("id");

        builder.Property(i => i.InvoiceNumber).HasMaxLength(32).IsRequired().HasColumnName("invoice_number");
        builder.Property(i => i.CustomerId).IsRequired().HasColumnName("customer_id");
        builder.Property(i => i.SiteId).HasColumnName("site_id");
        builder.Property(i => i.InvoiceType).HasConversion<string>().HasMaxLength(32).IsRequired().HasColumnName("invoice_type");
        builder.Property(i => i.AmcContractTermId).HasColumnName("amc_contract_term_id");
        builder.Property(i => i.TicketId).HasColumnName("ticket_id");
        builder.Property(i => i.ServiceVisitId).HasColumnName("service_visit_id");
        builder.Property(i => i.InvoiceDate).IsRequired().HasColumnName("invoice_date");
        builder.Property(i => i.DueDate).HasColumnName("due_date");
        builder.Property(i => i.SubtotalAmount).HasPrecision(18, 2).IsRequired().HasColumnName("subtotal_amount");
        builder.Property(i => i.TaxAmount).HasPrecision(18, 2).IsRequired().HasColumnName("tax_amount");
        builder.Property(i => i.TotalAmount).HasPrecision(18, 2).IsRequired().HasColumnName("total_amount");
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(32).IsRequired().HasColumnName("status");
        builder.Property(i => i.PaidAtUtc).HasColumnName("paid_at");
        builder.Property(i => i.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(i => i.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(i => i.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(i => i.UpdatedBy).HasColumnName("updated_by");
        builder.Property(i => i.RowVersion).HasColumnName("row_version").IsConcurrencyToken();

        builder.HasIndex(i => i.InvoiceNumber).IsUnique().HasDatabaseName("ux_invoices_invoice_number");
        builder.HasIndex(i => new { i.CustomerId, i.Status }).HasDatabaseName("ix_invoices_customer_id_status");
        builder.HasIndex(i => i.AmcContractTermId).HasDatabaseName("ix_invoices_amc_contract_term_id");

        builder.HasMany(i => i.Lines).WithOne().HasForeignKey(l => l.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(i => i.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(i => i.Attachments).WithOne().HasForeignKey(a => a.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(i => i.Attachments).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(i => i.StatusHistory).WithOne().HasForeignKey(h => h.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(i => i.StatusHistory).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(i => i.DomainEvents);
        builder.Ignore(i => i.InvoicePdfAttachment);
    }
}
