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
using PlanAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Plan.AmcPlan;

namespace Ashraak.Cctv.Amc.Application.Commands.CreatePlan;

internal sealed class CreatePlanCommandHandler(
    IAmcPlanRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CreatePlanCommand, Result<AmcPlanDetailDto>>
{
    public async Task<Result<AmcPlanDetailDto>> Handle(
        CreatePlanCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Amc.Disabled", "AMC management is not enabled for this tenant.");

        var authError = await AmcPlanAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var existing = await repository.GetByCodeAsync(request.Code.Trim(), cancellationToken);
        if (existing is not null)
            return Error.Conflict("AmcPlans.DuplicateCode", "An AMC plan with this code already exists.");

        var plan = PlanAggregate.Create(
            AmcPlanId.New(),
            request.Code,
            request.Name,
            request.Description,
            request.UserId);

        repository.Add(plan);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AmcMapper.ToPlanDetail(plan);
    }
}
