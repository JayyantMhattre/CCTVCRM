namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>AMC contract term summary within contract detail.</summary>
public sealed record AmcContractTermSummaryDto(
    Guid Id,
    int TermNo,
    Guid PlanVersionId,
    string PlanCode,
    int PlanVersionNo,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal AgreedPrice,
    string Status,
    string Origin,
    bool RenewalRequestedByCustomer,
    DateTime? RenewalRequestedAtUtc,
    uint RowVersion);
