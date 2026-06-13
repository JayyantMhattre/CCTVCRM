using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket.Events;

public sealed record TicketCreatedDomainEvent(
    Guid TicketId,
    string TicketNumber,
    Guid CustomerId,
    Guid SiteId,
    string Source,
    Guid CreatedBy) : DomainEvent;

public sealed record TicketAssignedDomainEvent(
    Guid TicketId,
    Guid EngineerId,
    Guid AssignedBy,
    bool IsReassignment) : DomainEvent;

public sealed record TicketStatusChangedDomainEvent(
    Guid TicketId,
    string FromStatus,
    string ToStatus,
    Guid ChangedBy,
    string? Reason) : DomainEvent;

public sealed record TicketClosedDomainEvent(
    Guid TicketId,
    Guid ClosedBy) : DomainEvent;

public sealed record TicketReopenedDomainEvent(
    Guid TicketId,
    string Reason,
    int ReopenCount,
    Guid ReopenedBy) : DomainEvent;
