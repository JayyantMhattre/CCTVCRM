namespace Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;

public readonly record struct TicketStatusHistoryId(Guid Value)
{
    public static TicketStatusHistoryId New() => new(Guid.NewGuid());

    public static TicketStatusHistoryId From(Guid value) => new(value);
}
