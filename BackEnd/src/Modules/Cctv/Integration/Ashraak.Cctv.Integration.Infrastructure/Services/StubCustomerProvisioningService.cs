using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Microsoft.Extensions.Logging;

namespace Ashraak.Cctv.Integration.Infrastructure.Services;

/// <summary>B1 stub — returns mock customer IDs until B2 Customer module is implemented.</summary>
internal sealed class StubCustomerProvisioningService(ILogger<StubCustomerProvisioningService> logger)
    : ICustomerProvisioningService
{
    public Task<CustomerProvisioningResultDto> ProvisionFromLeadAsync(
        CustomerProvisioningRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var customerId = Guid.NewGuid();
        logger.LogInformation(
            "Customer provisioning stub: lead {LeadId} -> customer {CustomerId}",
            request.LeadId,
            customerId);
        return Task.FromResult(new CustomerProvisioningResultDto(customerId));
    }
}
