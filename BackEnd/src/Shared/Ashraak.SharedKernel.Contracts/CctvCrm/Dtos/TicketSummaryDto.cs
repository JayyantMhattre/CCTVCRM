namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Ticket list row.</summary>
public sealed record TicketSummaryDto(
    Guid Id,
    string TicketNumber,
    Guid CustomerId,
    Guid SiteId,
    string Subject,
    string Priority,
    string Status,
    Guid? AssignedEngineerId,
    DateTime CreatedAtUtc,
    DateTime? ResolvedAtUtc,
    DateTime? ClosedAtUtc,
    int ReopenCount);
