using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Ashraak.Cctv.Ticket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketAggregate = Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket.Ticket;

namespace Ashraak.Cctv.Ticket.Infrastructure.Persistence.Configurations;

internal sealed class TicketConfiguration : IEntityTypeConfiguration<TicketAggregate>
{
    public void Configure(EntityTypeBuilder<TicketAggregate> builder)
    {
        builder.ToTable("tickets");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, value => TicketId.From(value))
            .HasColumnName("id");

        builder.Property(t => t.TicketNumber).HasMaxLength(32).IsRequired().HasColumnName("ticket_number");
        builder.Property(t => t.CustomerId).IsRequired().HasColumnName("customer_id");
        builder.Property(t => t.SiteId).IsRequired().HasColumnName("site_id");
        builder.Property(t => t.AmcContractId).HasColumnName("amc_contract_id");
        builder.Property(t => t.OriginServiceVisitId).HasColumnName("origin_service_visit_id");
        builder.Property(t => t.Source).HasConversion<string>().HasMaxLength(32).IsRequired().HasColumnName("source");
        builder.Property(t => t.Subject).HasMaxLength(200).IsRequired().HasColumnName("subject");
        builder.Property(t => t.Description).HasMaxLength(4000).IsRequired().HasColumnName("description");
        builder.Property(t => t.Priority).HasConversion<string>().HasMaxLength(32).IsRequired().HasColumnName("priority");
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(32).IsRequired().HasColumnName("status");
        builder.Property(t => t.ResolvedAtUtc).HasColumnName("resolved_at");
        builder.Property(t => t.ClosedAtUtc).HasColumnName("closed_at");
        builder.Property(t => t.ReopenCount).IsRequired().HasColumnName("reopen_count");
        builder.Property(t => t.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(t => t.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(t => t.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(t => t.UpdatedBy).HasColumnName("updated_by");
        builder.Property(t => t.IsDeleted).IsRequired().HasColumnName("is_deleted");
        builder.Property(t => t.RowVersion).HasColumnName("row_version").IsConcurrencyToken();

        builder.HasIndex(t => t.TicketNumber).IsUnique().HasDatabaseName("ux_tickets_ticket_number");
        builder.HasIndex(t => new { t.CustomerId, t.Status }).HasDatabaseName("ix_tickets_customer_id_status");
        builder.HasIndex(t => t.SiteId).HasDatabaseName("ix_tickets_site_id");

        builder.HasMany(t => t.Comments).WithOne().HasForeignKey(c => c.TicketId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(t => t.Comments).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(t => t.Attachments).WithOne().HasForeignKey(a => a.TicketId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(t => t.Attachments).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(t => t.Assignments).WithOne().HasForeignKey(a => a.TicketId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(t => t.Assignments).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(t => t.StatusHistory).WithOne().HasForeignKey(h => h.TicketId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(t => t.StatusHistory).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(t => t.DomainEvents);
        builder.Ignore(t => t.ActiveAssignment);
        builder.Ignore(t => t.ActiveAttachmentCount);
    }
}
