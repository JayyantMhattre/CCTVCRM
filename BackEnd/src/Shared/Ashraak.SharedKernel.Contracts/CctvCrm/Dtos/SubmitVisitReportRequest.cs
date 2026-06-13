namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/visits/{visitId}/submit.</summary>
public sealed record SubmitVisitReportRequest(
    uint RowVersion,
    string? ClientCorrelationId);
