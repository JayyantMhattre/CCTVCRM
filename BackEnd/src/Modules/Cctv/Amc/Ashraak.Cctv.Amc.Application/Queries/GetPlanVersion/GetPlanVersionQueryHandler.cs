using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.GetPlanVersion;

internal sealed class GetPlanVersionQueryHandler(
    IAmcPlanRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetPlanVersionQuery, Result<AmcPlanVersionDetailDto>>
{
    public async Task<Result<AmcPlanVersionDetailDto>> Handle(
        GetPlanVersionQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Amc.Disabled", "AMC management is not enabled for this tenant.");

        var authError = await AmcPlanAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var plan = await repository.GetByIdAsync(AmcPlanId.From(request.PlanId), cancellationToken);
        if (plan is null)
            return Error.NotFound("AmcPlans.NotFound", "AMC plan not found.");

        try
        {
            var version = plan.GetVersion(AmcPlanVersionId.From(request.VersionId));
            return AmcMapper.ToPlanVersionDetail(plan, version);
        }
        catch (InvalidOperationException)
        {
            return Error.NotFound("AmcPlans.VersionNotFound", "AMC plan version not found.");
        }
    }
}
