using Ashraak.Cctv.Engineer.Application.Mapping;
using Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer;
using Ashraak.Cctv.Engineer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Engineer.Application.Commands.UpdateEngineer;

internal sealed class UpdateEngineerCommandHandler(
    IEngineerRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateEngineerCommand, Result<EngineerDetailDto>>
{
    public async Task<Result<EngineerDetailDto>> Handle(
        UpdateEngineerCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.EngineersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Engineers.Disabled", "Engineer management is not enabled for this tenant.");

        var authError = await EngineerAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var engineer = await repository.GetByIdAsync(EngineerId.From(request.EngineerId), cancellationToken);
        if (engineer is null)
            return Error.NotFound("Engineers.NotFound", "Engineer not found.");

        var concurrencyError = EngineerConcurrencyHelper.EnsureRowVersion(engineer, request.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        if (request.PlatformUserId.HasValue)
        {
            var exists = await repository.ExistsByPlatformUserIdAsync(
                request.PlatformUserId.Value,
                engineer.Id,
                cancellationToken);
            if (exists)
            {
                return Error.Conflict(
                    "Engineers.DuplicatePlatformUser",
                    "Another engineer is already linked to this platform user.");
            }
        }

        engineer.Update(request.Name, request.Phone, request.PlatformUserId, request.UserId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return EngineerMapper.ToDetail(engineer);
    }
}
