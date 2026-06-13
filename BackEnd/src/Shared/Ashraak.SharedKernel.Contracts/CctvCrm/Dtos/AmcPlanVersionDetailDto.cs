namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>AMC plan version detail (GET /cctv/amc-plans/{planId}/versions/{versionId}).</summary>
public sealed record AmcPlanVersionDetailDto(
    Guid Id,
    Guid PlanId,
    string PlanCode,
    string PlanName,
    int VersionNo,
    decimal Price,
    int VisitFrequencyPerYear,
    IReadOnlyList<string> IncludedServices,
    string SlaTerms,
    DateOnly EffectiveFrom,
    string Status,
    bool IsReferenced,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
