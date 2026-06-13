using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;

/// <summary>Ticket file attachment metadata (platform FileRecord reference only).</summary>
public sealed class TicketAttachment : Entity<TicketAttachmentId>
{
    private TicketAttachment(TicketAttachmentId id) : base(id) { }

    public TicketId TicketId { get; private set; }
    public Guid FileId { get; private set; }
    public string? Title { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public bool IsDeleted { get; private set; }

    internal static TicketAttachment Create(
        TicketAttachmentId id,
        TicketId ticketId,
        Guid fileId,
        string? title,
        Guid createdBy)
    {
        return new TicketAttachment(id)
        {
            TicketId = ticketId,
            FileId = fileId,
            Title = title?.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy,
            IsDeleted = false
        };
    }

    internal void SoftDelete(Guid deletedBy)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
    }
}
