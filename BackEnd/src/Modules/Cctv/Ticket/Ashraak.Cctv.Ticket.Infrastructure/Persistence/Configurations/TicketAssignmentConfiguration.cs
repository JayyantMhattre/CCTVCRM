using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Ticket.Infrastructure.Persistence.Configurations;

internal sealed class TicketAssignmentConfiguration : IEntityTypeConfiguration<TicketAssignment>
{
    public void Configure(EntityTypeBuilder<TicketAssignment> builder)
    {
        builder.ToTable("ticket_assignments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasConversion(id => id.Value, value => TicketAssignmentId.From(value))
            .HasColumnName("id");

        builder.Property(a => a.TicketId)
            .HasConversion(id => id.Value, value => TicketId.From(value))
            .HasColumnName("ticket_id");
        builder.Property(a => a.EngineerId).IsRequired().HasColumnName("engineer_id");
        builder.Property(a => a.AssignedBy).IsRequired().HasColumnName("assigned_by");
        builder.Property(a => a.AssignedAtUtc).IsRequired().HasColumnName("assigned_at");
        builder.Property(a => a.IsActive).IsRequired().HasColumnName("is_active");
        builder.Property(a => a.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(a => a.CreatedBy).IsRequired().HasColumnName("created_by");

        builder.HasIndex(a => a.EngineerId).HasDatabaseName("ix_ticket_assignments_engineer_id");
        builder.HasIndex(a => a.TicketId)
            .IsUnique()
            .HasFilter("is_active = true")
            .HasDatabaseName("ux_ticket_assignments_ticket_active");
    }
}
