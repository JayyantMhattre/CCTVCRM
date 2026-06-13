namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>AMC contract list row (GET /cctv/contracts).</summary>
public sealed record AmcContractSummaryDto(
    Guid Id,
    string ContractNumber,
    Guid SiteId,
    Guid CustomerId,
    string Status,
    Guid? ActiveTermId,
    DateOnly? ActiveTermEndDate,
    string? PlanCode,
    DateTime CreatedAtUtc);
