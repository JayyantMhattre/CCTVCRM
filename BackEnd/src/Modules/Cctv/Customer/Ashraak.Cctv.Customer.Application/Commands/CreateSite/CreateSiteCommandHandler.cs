using Ashraak.Cctv.Customer.Application.Abstractions;
using Ashraak.Cctv.Customer.Application.Mapping;
using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using SiteAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Site.Site;

namespace Ashraak.Cctv.Customer.Application.Commands.CreateSite;

internal sealed class CreateSiteCommandHandler(
    ISiteRepository siteRepository,
    ICustomerRepository customerRepository,
    ISiteNumberGenerator numberGenerator,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateSiteCommand, Result<SiteDetailDto>>
{
    public async Task<Result<SiteDetailDto>> Handle(
        CreateSiteCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.CustomersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Sites.Disabled", "Site management is not enabled for this tenant.");

        var authError = await SiteAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var customer = await customerRepository.GetByIdAsync(CustomerId.From(request.CustomerId), cancellationToken);
        if (customer is null)
            return Error.NotFound("Sites.CustomerNotFound", "Customer not found.");

        var siteNumber = await numberGenerator.GenerateNextAsync(cancellationToken);
        var site = SiteAggregate.CreateManual(
            SiteId.New(),
            CustomerId.From(request.CustomerId),
            siteNumber,
            request.Name,
            request.Address,
            request.City,
            request.UserId);

        siteRepository.Add(site);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return SiteMapper.ToDetail(site);
    }
}
