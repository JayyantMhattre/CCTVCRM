namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Invoice status transition row.</summary>
public sealed record InvoiceStatusHistoryDto(
    Guid Id,
    string? FromStatus,
    string ToStatus,
    DateTime ChangedAtUtc,
    Guid ChangedBy);
