namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Result of lead conversion (POST /cctv/leads/{id}/convert).</summary>
public sealed record LeadConversionResultDto(
    Guid CustomerId,
    Guid SiteId,
    Guid ContractId,
    Guid TermId);
