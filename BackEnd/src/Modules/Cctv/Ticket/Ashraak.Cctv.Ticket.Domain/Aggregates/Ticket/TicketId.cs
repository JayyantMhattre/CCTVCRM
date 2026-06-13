namespace Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;

/// <summary>Strongly typed ticket identifier.</summary>
public readonly record struct TicketId(Guid Value)
{
    public static TicketId New() => new(Guid.NewGuid());

    public static TicketId From(Guid value) => new(value);
}
