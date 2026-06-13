namespace Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;

public readonly record struct TicketAttachmentId(Guid Value)
{
    public static TicketAttachmentId New() => new(Guid.NewGuid());

    public static TicketAttachmentId From(Guid value) => new(value);
}
