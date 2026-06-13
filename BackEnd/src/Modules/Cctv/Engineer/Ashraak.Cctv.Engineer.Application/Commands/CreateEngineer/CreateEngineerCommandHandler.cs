using Ashraak.Cctv.Engineer.Application.Abstractions;
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
using EngineerAggregate = Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer.Engineer;

namespace Ashraak.Cctv.Engineer.Application.Commands.CreateEngineer;

internal sealed class CreateEngineerCommandHandler(
    IEngineerRepository repository,
    IEngineerNumberGenerator numberGenerator,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateEngineerCommand, Result<EngineerDetailDto>>
{
    public async Task<Result<EngineerDetailDto>> Handle(
        CreateEngineerCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.EngineersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Engineers.Disabled", "Engineer management is not enabled for this tenant.");

        var authError = await EngineerAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        if (request.PlatformUserId.HasValue)
        {
            var exists = await repository.ExistsByPlatformUserIdAsync(
                request.PlatformUserId.Value,
                null,
                cancellationToken);
            if (exists)
            {
                return Error.Conflict(
                    "Engineers.DuplicatePlatformUser",
                    "Another engineer is already linked to this platform user.");
            }
        }

        var engineerNumber = await numberGenerator.GenerateNextAsync(cancellationToken);
        var engineer = EngineerAggregate.Create(
            EngineerId.New(),
            engineerNumber,
            request.Name,
            request.Phone,
            request.PlatformUserId,
            request.UserId);

        repository.Add(engineer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return EngineerMapper.ToDetail(engineer);
    }
}
