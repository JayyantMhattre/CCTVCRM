using Ashraak.Cctv.Engineer.Application.Mapping;
using Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer;
using Ashraak.Cctv.Engineer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Engineer.Application.Queries.GetEngineer;

internal sealed class GetEngineerQueryHandler(
    IEngineerRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetEngineerQuery, Result<EngineerDetailDto>>
{
    public async Task<Result<EngineerDetailDto>> Handle(
        GetEngineerQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.EngineersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Engineers.Disabled", "Engineer management is not enabled for this tenant.");

        var authError = await EngineerAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var engineer = await repository.GetByIdAsync(EngineerId.From(request.EngineerId), cancellationToken);
        if (engineer is null)
            return Error.NotFound("Engineers.NotFound", "Engineer not found.");

        return EngineerMapper.ToDetail(engineer);
    }
}
