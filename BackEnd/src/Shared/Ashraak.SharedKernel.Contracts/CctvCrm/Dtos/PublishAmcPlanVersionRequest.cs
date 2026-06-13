namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/amc-plans/{planId}/versions/{versionId}/publish.</summary>
public sealed record PublishAmcPlanVersionRequest(uint RowVersion);
