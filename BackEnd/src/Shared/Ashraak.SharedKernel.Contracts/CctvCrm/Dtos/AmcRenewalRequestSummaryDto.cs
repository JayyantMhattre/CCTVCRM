namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Renewal request queue row (GET /cctv/renewal-requests).</summary>
public sealed record AmcRenewalRequestSummaryDto(
    Guid ContractId,
    string ContractNumber,
    Guid SiteId,
    Guid CustomerId,
    Guid TermId,
    int TermNo,
    DateOnly TermEndDate,
    DateTime RenewalRequestedAtUtc);
