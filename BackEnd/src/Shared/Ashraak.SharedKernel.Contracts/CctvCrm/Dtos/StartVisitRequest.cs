namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/visits/{visitId}/start.</summary>
public sealed record StartVisitRequest(
    DateTime? StartedAt,
    uint RowVersion);
