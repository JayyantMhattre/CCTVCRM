namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>AMC plan list row (GET /cctv/amc-plans).</summary>
public sealed record AmcPlanSummaryDto(
    Guid Id,
    string PlanCode,
    string Name,
    string? Description,
    string Status,
    int PublishedVersionCount,
    DateTime CreatedAtUtc);
