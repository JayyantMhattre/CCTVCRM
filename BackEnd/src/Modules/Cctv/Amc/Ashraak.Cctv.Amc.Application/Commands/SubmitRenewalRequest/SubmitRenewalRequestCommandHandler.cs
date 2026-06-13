using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.SubmitRenewalRequest;

internal sealed class SubmitRenewalRequestCommandHandler(
    IAmcPlanRepository planRepository,
    IAmcContractRepository contractRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<SubmitRenewalRequestCommand, Result<AmcContractDetailDto>>
{
    public async Task<Result<AmcContractDetailDto>> Handle(
        SubmitRenewalRequestCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Amc.Disabled", "AMC management is not enabled for this tenant.");

        var authError = await AmcContractAuthorization.EnsureCanRequestRenewalAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var contract = await contractRepository.GetByIdAsync(AmcContractId.From(request.ContractId), cancellationToken);
        if (contract is null)
            return Error.NotFound("Amc.NotFound", "AMC contract not found.");

        var activeTerm = contract.GetActiveTerm();
        if (activeTerm is null)
            return Error.Validation("Amc.NoActiveTerm", "No active term found for renewal request.");

        try
        {
            contract.RequestRenewal(activeTerm.Id, request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var meta = await AmcPlanVersionMetaHelper.BuildAsync(contract, planRepository, cancellationToken);

            return AmcMapper.ToContractDetail(contract, meta);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Amc.RenewalFailed", ex.Message);
        }
    }
}
