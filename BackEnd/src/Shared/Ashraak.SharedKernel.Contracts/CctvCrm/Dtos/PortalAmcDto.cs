namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Customer portal active AMC view (GET /cctv/portal/amc).</summary>
public sealed record PortalAmcDto(
    Guid ContractId,
    string ContractNumber,
    Guid SiteId,
    Guid TermId,
    int TermNo,
    string PlanCode,
    string PlanName,
    decimal AgreedPrice,
    int VisitFrequencyPerYear,
    IReadOnlyList<string> IncludedServices,
    string SlaTerms,
    DateOnly StartDate,
    DateOnly EndDate,
    string Status);
