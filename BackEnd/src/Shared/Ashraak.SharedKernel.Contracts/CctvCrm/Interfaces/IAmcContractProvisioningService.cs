using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Cross-module contract to provision an AMC contract from a converted lead (B3 implementation).</summary>
public interface IAmcContractProvisioningService
{
    Task<AmcContractProvisioningResultDto> ProvisionFromLeadAsync(
        AmcContractProvisioningRequestDto request,
        CancellationToken cancellationToken = default);
}
