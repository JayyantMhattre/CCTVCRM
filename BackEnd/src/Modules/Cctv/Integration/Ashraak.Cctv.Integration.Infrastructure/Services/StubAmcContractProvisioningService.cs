using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Microsoft.Extensions.Logging;

namespace Ashraak.Cctv.Integration.Infrastructure.Services;

/// <summary>B1 stub — returns mock contract/term IDs until B3 AMC module is implemented.</summary>
internal sealed class StubAmcContractProvisioningService(ILogger<StubAmcContractProvisioningService> logger)
    : IAmcContractProvisioningService
{
    public Task<AmcContractProvisioningResultDto> ProvisionFromLeadAsync(
        AmcContractProvisioningRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var contractId = Guid.NewGuid();
        var termId = Guid.NewGuid();
        logger.LogInformation(
            "AMC contract provisioning stub: lead {LeadId} site {SiteId} -> contract {ContractId} term {TermId}",
            request.LeadId,
            request.SiteId,
            contractId,
            termId);
        return Task.FromResult(new AmcContractProvisioningResultDto(contractId, termId));
    }
}
