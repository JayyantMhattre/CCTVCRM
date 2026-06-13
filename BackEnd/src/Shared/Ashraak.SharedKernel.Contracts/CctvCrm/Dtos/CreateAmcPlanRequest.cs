namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/amc-plans.</summary>
public sealed record CreateAmcPlanRequest(
    string Code,
    string Name,
    string? Description);
