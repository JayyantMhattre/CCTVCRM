using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.UpdatePlan;

internal sealed class UpdatePlanCommandHandler(
    IAmcPlanRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdatePlanCommand, Result<AmcPlanDetailDto>>
{
    public async Task<Result<AmcPlanDetailDto>> Handle(
        UpdatePlanCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Amc.Disabled", "AMC management is not enabled for this tenant.");

        var authError = await AmcPlanAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var plan = await repository.GetByIdAsync(AmcPlanId.From(request.PlanId), cancellationToken);
        if (plan is null)
            return Error.NotFound("AmcPlans.NotFound", "AMC plan not found.");

        var concurrencyError = AmcConcurrencyHelper.EnsureRowVersion(request.RowVersion, plan.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        plan.UpdateIdentity(request.Name, request.Description, request.UserId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AmcMapper.ToPlanDetail(plan);
    }
}
