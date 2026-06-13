namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/visits/{visitId}/return.</summary>
public sealed record ReturnVisitRequest(
    string ReturnReason);
