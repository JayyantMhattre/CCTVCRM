using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.GetPortalAmc;

internal sealed class GetPortalAmcQueryHandler(
    IAmcContractRepository contractRepository,
    IAmcPlanRepository planRepository,
    ICustomerLookupService customerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetPortalAmcQuery, Result<PortalAmcDto>>
{
    public async Task<Result<PortalAmcDto>> Handle(
        GetPortalAmcQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Amc.Disabled", "AMC management is not enabled for this tenant.");

        var authError = await AmcContractAuthorization.EnsureCanAccessPortalAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var customer = await customerLookup.GetCustomerForUserAsync(request.UserId, cancellationToken);
        if (customer is null)
            return Error.NotFound("Amc.PortalCustomerNotFound", "No customer profile linked to this user.");

        var contract = await contractRepository.GetByCustomerIdWithActiveTermAsync(customer.Id, cancellationToken);
        if (contract is null)
            return Error.NotFound("Amc.PortalNotFound", "No active AMC contract found for your account.");

        var activeTerm = contract.GetActiveTerm();
        if (activeTerm is null)
            return Error.NotFound("Amc.PortalNotFound", "No active AMC term found.");

        var version = await planRepository.GetVersionByIdAsync(activeTerm.PlanVersionId, cancellationToken);
        if (version is null)
            return Error.NotFound("AmcPlans.VersionNotFound", "Plan version not found.");

        var plan = await planRepository.GetByIdAsync(version.PlanId, cancellationToken);
        if (plan is null)
            return Error.NotFound("AmcPlans.NotFound", "AMC plan not found.");

        return AmcMapper.ToPortalAmc(contract, plan, version, activeTerm);
    }
}
