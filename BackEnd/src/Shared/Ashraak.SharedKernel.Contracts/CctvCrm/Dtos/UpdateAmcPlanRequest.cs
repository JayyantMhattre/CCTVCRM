namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>PUT /cctv/amc-plans/{id}.</summary>
public sealed record UpdateAmcPlanRequest(
    string Name,
    string? Description,
    uint RowVersion);
