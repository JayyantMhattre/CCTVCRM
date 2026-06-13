namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>AMC plan detail with versions (GET /cctv/amc-plans/{id}).</summary>
public sealed record AmcPlanDetailDto(
    Guid Id,
    string PlanCode,
    string Name,
    string? Description,
    string Status,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    DateTime? UpdatedAtUtc,
    Guid? UpdatedBy,
    uint RowVersion,
    IReadOnlyList<AmcPlanVersionSummaryDto> Versions);
