namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>AMC plan version summary within a plan detail.</summary>
public sealed record AmcPlanVersionSummaryDto(
    Guid Id,
    int VersionNo,
    decimal Price,
    int VisitFrequencyPerYear,
    DateOnly EffectiveFrom,
    string Status,
    DateTime CreatedAtUtc);
