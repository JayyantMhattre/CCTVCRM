using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Ashraak.Cctv.Ticket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Ticket.Infrastructure.Persistence.Configurations;

internal sealed class TicketCommentConfiguration : IEntityTypeConfiguration<TicketComment>
{
    public void Configure(EntityTypeBuilder<TicketComment> builder)
    {
        builder.ToTable("ticket_comments");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => TicketCommentId.From(value))
            .HasColumnName("id");

        builder.Property(c => c.TicketId)
            .HasConversion(id => id.Value, value => TicketId.From(value))
            .HasColumnName("ticket_id");
        builder.Property(c => c.Comment).HasMaxLength(4000).IsRequired().HasColumnName("comment");
        builder.Property(c => c.AuthorRole).HasConversion<string>().HasMaxLength(32).IsRequired().HasColumnName("author_role");
        builder.Property(c => c.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(c => c.CreatedBy).IsRequired().HasColumnName("created_by");
    }
}
