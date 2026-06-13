namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Activity timeline row.</summary>
public sealed record LeadActivityDto(
    Guid Id,
    Guid LeadId,
    string ActivityType,
    string? FromStatus,
    string? ToStatus,
    string Description,
    DateTime OccurredAtUtc,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
