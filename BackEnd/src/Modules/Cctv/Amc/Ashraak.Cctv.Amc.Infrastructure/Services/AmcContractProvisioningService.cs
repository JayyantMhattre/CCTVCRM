using Ashraak.Cctv.Amc.Application.Abstractions;
using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Enums;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using ContractAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Contract.AmcContract;

namespace Ashraak.Cctv.Amc.Infrastructure.Services;

/// <summary>Real AMC contract provisioning from lead conversion (replaces Integration stub).</summary>
internal sealed class AmcContractProvisioningService(
    IAmcPlanRepository planRepository,
    IAmcContractRepository contractRepository,
    IAmcContractNumberGenerator numberGenerator,
    IUnitOfWork unitOfWork) : IAmcContractProvisioningService
{
    public async Task<AmcContractProvisioningResultDto> ProvisionFromLeadAsync(
        AmcContractProvisioningRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingActive = await contractRepository.GetActiveBySiteIdAsync(request.SiteId, cancellationToken);
        if (existingActive is not null)
            throw new InvalidOperationException("An active AMC contract already exists for this site.");

        var version = await planRepository.GetVersionByIdAsync(
            AmcPlanVersionId.From(request.PlanVersionId),
            cancellationToken);

        if (version is null || version.Status != PlanVersionStatus.Published)
            throw new InvalidOperationException("A published plan version is required for lead conversion.");

        var contractNumber = await numberGenerator.GenerateNextAsync(cancellationToken);
        var contract = ContractAggregate.Create(
            AmcContractId.New(),
            contractNumber,
            request.SiteId,
            request.CustomerId,
            request.LeadId,
            Guid.Empty);

        var termId = AmcContractTermId.New();
        contract.AddTerm(
            termId,
            version.Id,
            request.InitialTermStartDate,
            request.InitialTermEndDate,
            version.Price,
            TermOrigin.New,
            Guid.Empty);

        version.MarkReferenced();
        contract.ActivateTerm(termId, Guid.Empty);

        contractRepository.Add(contract);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AmcContractProvisioningResultDto(contract.Id.Value, termId.Value);
    }
}
