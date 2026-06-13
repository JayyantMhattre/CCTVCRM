namespace Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;

public readonly record struct TicketAssignmentId(Guid Value)
{
    public static TicketAssignmentId New() => new(Guid.NewGuid());

    public static TicketAssignmentId From(Guid value) => new(value);
}
