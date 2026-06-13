using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Ashraak.Cctv.Ticket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Ticket.Infrastructure.Persistence.Configurations;

internal sealed class TicketStatusHistoryConfiguration : IEntityTypeConfiguration<TicketStatusHistory>
{
    public void Configure(EntityTypeBuilder<TicketStatusHistory> builder)
    {
        builder.ToTable("ticket_status_histories");

        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id)
            .HasConversion(id => id.Value, value => TicketStatusHistoryId.From(value))
            .HasColumnName("id");

        builder.Property(h => h.TicketId)
            .HasConversion(id => id.Value, value => TicketId.From(value))
            .HasColumnName("ticket_id");
        builder.Property(h => h.FromStatus).HasConversion<string>().HasMaxLength(32).HasColumnName("from_status");
        builder.Property(h => h.ToStatus).HasConversion<string>().HasMaxLength(32).IsRequired().HasColumnName("to_status");
        builder.Property(h => h.Reason).HasMaxLength(2000).HasColumnName("reason");
        builder.Property(h => h.ChangedAtUtc).IsRequired().HasColumnName("changed_at");
        builder.Property(h => h.ChangedBy).IsRequired().HasColumnName("changed_by");
        builder.Property(h => h.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(h => h.CreatedBy).IsRequired().HasColumnName("created_by");

        builder.HasIndex(h => h.TicketId).HasDatabaseName("ix_ticket_status_histories_ticket_id");
    }
}
