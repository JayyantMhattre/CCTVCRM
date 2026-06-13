namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/amc-plans/{planId}/versions.</summary>
public sealed record CreateAmcPlanVersionRequest(
    decimal Price,
    int VisitFrequency,
    IReadOnlyList<string> IncludedServices,
    string SlaDescription,
    DateOnly EffectiveFrom);
