namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/contracts/{contractId}/terms.</summary>
public sealed record CreateAmcContractTermRequest(
    Guid PlanVersionId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Price,
    string TermType);
