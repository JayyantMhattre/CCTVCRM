namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>PATCH /cctv/contracts/{contractId}/status (cancel).</summary>
public sealed record CancelAmcContractRequest(uint RowVersion);
