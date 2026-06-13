using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.ActivateContractTerm;

internal sealed class ActivateContractTermCommandHandler(
    IAmcPlanRepository planRepository,
    IAmcContractRepository contractRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<ActivateContractTermCommand, Result<AmcContractDetailDto>>
{
    public async Task<Result<AmcContractDetailDto>> Handle(
        ActivateContractTermCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Amc.Disabled", "AMC management is not enabled for this tenant.");

        var authError = await AmcContractAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var contract = await contractRepository.GetByIdAsync(AmcContractId.From(request.ContractId), cancellationToken);
        if (contract is null)
            return Error.NotFound("Amc.NotFound", "AMC contract not found.");

        var term = contract.GetTerm(AmcContractTermId.From(request.TermId));
        var concurrencyError = AmcConcurrencyHelper.EnsureRowVersion(request.RowVersion, term.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        try
        {
            contract.ActivateTerm(AmcContractTermId.From(request.TermId), request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var meta = await AmcPlanVersionMetaHelper.BuildAsync(contract, planRepository, cancellationToken);
            return AmcMapper.ToContractDetail(contract, meta);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Amc.ActivateTermFailed", ex.Message);
        }
    }
}
