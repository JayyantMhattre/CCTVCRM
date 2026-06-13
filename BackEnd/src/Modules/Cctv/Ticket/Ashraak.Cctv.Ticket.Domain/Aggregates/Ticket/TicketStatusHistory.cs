using Ashraak.Cctv.Ticket.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;

/// <summary>Append-only status transition audit row.</summary>
public sealed class TicketStatusHistory : Entity<TicketStatusHistoryId>
{
    private TicketStatusHistory(TicketStatusHistoryId id) : base(id) { }

    public TicketId TicketId { get; private set; }
    public TicketStatus? FromStatus { get; private set; }
    public TicketStatus ToStatus { get; private set; }
    public string? Reason { get; private set; }
    public DateTime ChangedAtUtc { get; private set; }
    public Guid ChangedBy { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    internal static TicketStatusHistory Create(
        TicketStatusHistoryId id,
        TicketId ticketId,
        TicketStatus? fromStatus,
        TicketStatus toStatus,
        string? reason,
        Guid changedBy)
    {
        return new TicketStatusHistory(id)
        {
            TicketId = ticketId,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            Reason = reason?.Trim(),
            ChangedAtUtc = DateTime.UtcNow,
            ChangedBy = changedBy,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = changedBy
        };
    }
}
