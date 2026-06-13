using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.GetContract;

internal sealed class GetContractQueryHandler(
    IAmcContractRepository contractRepository,
    IAmcPlanRepository planRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetContractQuery, Result<AmcContractDetailDto>>
{
    public async Task<Result<AmcContractDetailDto>> Handle(
        GetContractQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Amc.Disabled", "AMC management is not enabled for this tenant.");

        var authError = await AmcContractAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var contract = await contractRepository.GetByIdAsync(AmcContractId.From(request.ContractId), cancellationToken);
        if (contract is null)
            return Error.NotFound("Amc.NotFound", "AMC contract not found.");

        var meta = await AmcPlanVersionMetaHelper.BuildAsync(contract, planRepository, cancellationToken);

        return AmcMapper.ToContractDetail(contract, meta);
    }
}
