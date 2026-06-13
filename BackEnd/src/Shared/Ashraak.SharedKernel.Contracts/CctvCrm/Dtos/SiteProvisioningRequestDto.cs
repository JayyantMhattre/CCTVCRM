namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Input for site provisioning from a converted lead.</summary>
public sealed record SiteProvisioningRequestDto(
    Guid LeadId,
    Guid CustomerId,
    string SiteName,
    string SiteAddress,
    string City);
