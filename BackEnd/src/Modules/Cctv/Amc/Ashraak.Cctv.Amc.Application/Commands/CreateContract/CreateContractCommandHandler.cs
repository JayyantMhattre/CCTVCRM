using Ashraak.Cctv.Amc.Application.Abstractions;
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
using ContractAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Contract.AmcContract;

namespace Ashraak.Cctv.Amc.Application.Commands.CreateContract;

internal sealed class CreateContractCommandHandler(
    IAmcPlanRepository planRepository,
    IAmcContractRepository contractRepository,
    IAmcContractNumberGenerator numberGenerator,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateContractCommand, Result<AmcContractDetailDto>>
{
    public async Task<Result<AmcContractDetailDto>> Handle(
        CreateContractCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Amc.Disabled", "AMC management is not enabled for this tenant.");

        var authError = await AmcContractAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var existingActive = await contractRepository.GetActiveBySiteIdAsync(request.SiteId, cancellationToken);
        if (existingActive is not null)
            return Error.Conflict("Amc.ActiveContractExists", "An active AMC contract already exists for this site.");

        var version = await planRepository.GetVersionByIdAsync(AmcPlanVersionId.From(request.PlanVersionId), cancellationToken);
        if (version is null || version.Status != PlanVersionStatus.Published)
            return Error.Validation("Amc.PublishedVersionRequired", "A published plan version is required.");

        var plan = await planRepository.GetByIdAsync(version.PlanId, cancellationToken);
        if (plan is null)
            return Error.NotFound("AmcPlans.NotFound", "AMC plan not found.");

        try
        {
            var contractNumber = await numberGenerator.GenerateNextAsync(cancellationToken);
            var contract = ContractAggregate.Create(
                AmcContractId.New(),
                contractNumber,
                request.SiteId,
                request.CustomerId,
                null,
                request.UserId);

            var termId = AmcContractTermId.New();
            contract.AddTerm(
                termId,
                version.Id,
                request.StartDate,
                request.EndDate,
                request.Price,
                TermOrigin.New,
                request.UserId);

            version.MarkReferenced();
            contract.ActivateTerm(termId, request.UserId);

            contractRepository.Add(contract);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var meta = new Dictionary<Guid, (string PlanCode, int VersionNo)>
            {
                [version.Id.Value] = (plan.PlanCode, version.VersionNo)
            };

            return AmcMapper.ToContractDetail(contract, meta);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Amc.CreateContractFailed", ex.Message);
        }
    }
}
