namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>AMC contract term detail with pinned plan version snapshot.</summary>
public sealed record AmcContractTermDetailDto(
    Guid Id,
    Guid ContractId,
    int TermNo,
    Guid PlanVersionId,
    string PlanCode,
    int PlanVersionNo,
    decimal PlanPrice,
    int VisitFrequencyPerYear,
    IReadOnlyList<string> IncludedServices,
    string SlaTerms,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal AgreedPrice,
    string Status,
    string Origin,
    bool RenewalRequestedByCustomer,
    DateTime? RenewalRequestedAtUtc,
    uint RowVersion);
