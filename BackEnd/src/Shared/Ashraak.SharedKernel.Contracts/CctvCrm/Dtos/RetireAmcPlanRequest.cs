namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>PATCH /cctv/amc-plans/{id}/status (retire).</summary>
public sealed record RetireAmcPlanRequest(uint RowVersion);
