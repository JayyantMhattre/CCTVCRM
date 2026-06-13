namespace Ashraak.Cctv.Ticket.Domain.Enums;

/// <summary>Ticket lifecycle status (BR-TKT-01).</summary>
public enum TicketStatus
{
    Open = 0,
    Assigned = 1,
    InProgress = 2,
    Resolved = 3,
    Closed = 4,
    Reopened = 5
}
