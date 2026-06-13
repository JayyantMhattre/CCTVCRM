namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Offline sync per-item rejection.</summary>
public sealed record OfflineSyncRejectedItemDto(
    Guid VisitId,
    string? ClientCorrelationId,
    string ErrorCode,
    string ErrorMessage);

/// <summary>POST /cctv/engineer/visits/sync result.</summary>
public sealed record OfflineSyncResultDto(
    IReadOnlyList<Guid> Accepted,
    IReadOnlyList<OfflineSyncRejectedItemDto> Rejected);
