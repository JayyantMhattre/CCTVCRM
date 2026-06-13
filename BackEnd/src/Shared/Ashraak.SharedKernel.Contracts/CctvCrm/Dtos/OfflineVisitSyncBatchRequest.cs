namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Offline visit sync payload item for POST /cctv/engineer/visits/sync.</summary>
public sealed record OfflineVisitSyncItemRequest(
    Guid VisitId,
    string? ClientCorrelationId,
    uint RowVersion);

/// <summary>POST /cctv/engineer/visits/sync.</summary>
public sealed record OfflineVisitSyncBatchRequest(
    IReadOnlyList<OfflineVisitSyncItemRequest> Items);
