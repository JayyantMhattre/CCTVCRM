using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Cross-module contract to provision a customer from a converted lead (B2 implementation).</summary>
public interface ICustomerProvisioningService
{
    Task<CustomerProvisioningResultDto> ProvisionFromLeadAsync(
        CustomerProvisioningRequestDto request,
        CancellationToken cancellationToken = default);
}
