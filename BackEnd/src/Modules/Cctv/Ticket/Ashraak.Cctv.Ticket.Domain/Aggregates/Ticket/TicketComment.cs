using Ashraak.Cctv.Ticket.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;

/// <summary>Append-only ticket comment.</summary>
public sealed class TicketComment : Entity<TicketCommentId>
{
    private TicketComment(TicketCommentId id) : base(id) { }

    public TicketId TicketId { get; private set; }
    public string Comment { get; private set; } = string.Empty;
    public TicketAuthorRole AuthorRole { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    internal static TicketComment Create(
        TicketCommentId id,
        TicketId ticketId,
        string comment,
        TicketAuthorRole authorRole,
        Guid createdBy)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new InvalidOperationException("Comment text is required.");

        return new TicketComment(id)
        {
            TicketId = ticketId,
            Comment = comment.Trim(),
            AuthorRole = authorRole,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
