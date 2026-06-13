namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Ticket status transition row.</summary>
public sealed record TicketStatusHistoryDto(
    Guid Id,
    string? FromStatus,
    string ToStatus,
    string? Reason,
    DateTime ChangedAtUtc,
    Guid ChangedBy);
