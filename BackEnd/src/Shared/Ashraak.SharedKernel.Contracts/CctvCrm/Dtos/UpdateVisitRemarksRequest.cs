namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>PUT /cctv/visits/{visitId}/remarks.</summary>
public sealed record UpdateVisitRemarksRequest(
    string Remarks,
    uint RowVersion);
