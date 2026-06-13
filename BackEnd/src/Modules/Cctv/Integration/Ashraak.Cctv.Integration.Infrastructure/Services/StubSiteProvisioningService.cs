using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Microsoft.Extensions.Logging;

namespace Ashraak.Cctv.Integration.Infrastructure.Services;

/// <summary>B1 stub — returns mock site IDs until B2 Site module is implemented.</summary>
internal sealed class StubSiteProvisioningService(ILogger<StubSiteProvisioningService> logger)
    : ISiteProvisioningService
{
    public Task<SiteProvisioningResultDto> ProvisionFromLeadAsync(
        SiteProvisioningRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var siteId = Guid.NewGuid();
        logger.LogInformation(
            "Site provisioning stub: lead {LeadId} customer {CustomerId} -> site {SiteId}",
            request.LeadId,
            request.CustomerId,
            siteId);
        return Task.FromResult(new SiteProvisioningResultDto(siteId));
    }
}
