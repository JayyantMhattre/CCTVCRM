using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Ticket.Infrastructure.Persistence.Configurations;

internal sealed class TicketAttachmentConfiguration : IEntityTypeConfiguration<TicketAttachment>
{
    public void Configure(EntityTypeBuilder<TicketAttachment> builder)
    {
        builder.ToTable("ticket_attachments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasConversion(id => id.Value, value => TicketAttachmentId.From(value))
            .HasColumnName("id");

        builder.Property(a => a.TicketId)
            .HasConversion(id => id.Value, value => TicketId.From(value))
            .HasColumnName("ticket_id");
        builder.Property(a => a.FileId).IsRequired().HasColumnName("file_id");
        builder.Property(a => a.Title).HasMaxLength(200).HasColumnName("title");
        builder.Property(a => a.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(a => a.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(a => a.IsDeleted).IsRequired().HasColumnName("is_deleted");
    }
}
