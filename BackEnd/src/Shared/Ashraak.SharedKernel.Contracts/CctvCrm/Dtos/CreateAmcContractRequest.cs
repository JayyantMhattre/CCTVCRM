namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/contracts.</summary>
public sealed record CreateAmcContractRequest(
    Guid SiteId,
    Guid CustomerId,
    Guid PlanVersionId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Price);
