using Ashraak.Cctv.Customer.Application.Abstractions;
using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using SiteAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Site.Site;

namespace Ashraak.Cctv.Customer.Infrastructure.Services;

/// <summary>Real site provisioning from lead conversion (replaces Integration stub).</summary>
internal sealed class SiteProvisioningService(
    ISiteRepository repository,
    ISiteNumberGenerator numberGenerator,
    IUnitOfWork unitOfWork) : ISiteProvisioningService
{
    public async Task<SiteProvisioningResultDto> ProvisionFromLeadAsync(
        SiteProvisioningRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var siteNumber = await numberGenerator.GenerateNextAsync(cancellationToken);
        var site = SiteAggregate.CreateFromLead(
            SiteId.New(),
            CustomerId.From(request.CustomerId),
            siteNumber,
            request.SiteName.Trim(),
            request.SiteAddress.Trim(),
            request.City.Trim(),
            Guid.Empty);

        repository.Add(site);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SiteProvisioningResultDto(site.Id.Value);
    }
}
