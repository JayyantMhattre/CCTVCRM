using Ashraak.Cctv.Lead.Application.Abstractions;
using LeadAggregate = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.Lead;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Results;

namespace Ashraak.Cctv.Lead.Infrastructure.Services;

/// <summary>
/// B1 stub orchestrator — provisions mock IDs via stub services until B2/B3 implement real Customer/Site/AMC modules.
/// </summary>
internal sealed class LeadConversionOrchestrator(
    ICustomerProvisioningService customerProvisioning,
    ISiteProvisioningService siteProvisioning,
    IAmcContractProvisioningService contractProvisioning) : ILeadConversionOrchestrator
{
    public async Task<Result<LeadConversionResultDto>> ConvertAsync(
        LeadAggregate lead,
        ConvertLeadRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(lead);
        ArgumentNullException.ThrowIfNull(request);

        // B1: delegate to cross-module stub provisioning services (real orchestration in B2/B3).
        var customer = await customerProvisioning.ProvisionFromLeadAsync(
            new CustomerProvisioningRequestDto(
                lead.Id.Value,
                lead.ContactName,
                lead.OrganizationName,
                lead.Email,
                lead.Phone,
                lead.City,
                lead.Address),
            cancellationToken);

        var site = await siteProvisioning.ProvisionFromLeadAsync(
            new SiteProvisioningRequestDto(
                lead.Id.Value,
                customer.CustomerId,
                request.SiteName,
                request.SiteAddress,
                lead.City),
            cancellationToken);

        var contract = await contractProvisioning.ProvisionFromLeadAsync(
            new AmcContractProvisioningRequestDto(
                lead.Id.Value,
                customer.CustomerId,
                site.SiteId,
                request.PlanVersionId,
                request.InitialTermStartDate,
                request.InitialTermEndDate),
            cancellationToken);

        return new LeadConversionResultDto(
            customer.CustomerId,
            site.SiteId,
            contract.ContractId,
            contract.TermId);
    }
}
