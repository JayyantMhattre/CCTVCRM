namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/contracts/{contractId}/terms/{termId}/activate.</summary>
public sealed record ActivateAmcTermRequest(uint RowVersion);
