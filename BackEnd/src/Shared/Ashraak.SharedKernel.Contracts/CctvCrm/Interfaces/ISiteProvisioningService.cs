using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Cross-module contract to provision a site from a converted lead (B2 implementation).</summary>
public interface ISiteProvisioningService
{
    Task<SiteProvisioningResultDto> ProvisionFromLeadAsync(
        SiteProvisioningRequestDto request,
        CancellationToken cancellationToken = default);
}
