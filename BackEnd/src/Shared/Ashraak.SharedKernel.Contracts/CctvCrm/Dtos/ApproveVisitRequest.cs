namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/visits/{visitId}/approve.</summary>
public sealed record ApproveVisitRequest(
    string? ReviewRemarks);
