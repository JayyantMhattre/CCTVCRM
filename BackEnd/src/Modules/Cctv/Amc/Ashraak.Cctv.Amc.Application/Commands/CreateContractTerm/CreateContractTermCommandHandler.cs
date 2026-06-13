using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Enums;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.CreateContractTerm;

internal sealed class CreateContractTermCommandHandler(
    IAmcPlanRepository planRepository,
    IAmcContractRepository contractRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateContractTermCommand, Result<AmcContractTermDetailDto>>
{
    public async Task<Result<AmcContractTermDetailDto>> Handle(
        CreateContractTermCommand request,
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

        var version = await planRepository.GetVersionByIdAsync(AmcPlanVersionId.From(request.PlanVersionId), cancellationToken);
        if (version is null || version.Status != PlanVersionStatus.Published)
            return Error.Validation("Amc.PublishedVersionRequired", "A published plan version is required.");

        var plan = await planRepository.GetByIdAsync(version.PlanId, cancellationToken);
        if (plan is null)
            return Error.NotFound("AmcPlans.NotFound", "AMC plan not found.");

        try
        {
            var term = contract.AddTerm(
                AmcContractTermId.New(),
                version.Id,
                request.StartDate,
                request.EndDate,
                request.Price,
                AmcMapper.ParseTermOrigin(request.TermType),
                request.UserId);

            version.MarkReferenced();
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return AmcMapper.ToTermDetail(term, plan, version);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Amc.CreateTermFailed", ex.Message);
        }
    }
}
