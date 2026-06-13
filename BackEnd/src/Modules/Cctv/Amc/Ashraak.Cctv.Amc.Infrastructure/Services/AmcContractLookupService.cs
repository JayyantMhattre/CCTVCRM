using Ashraak.Cctv.Amc.Application;
using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

namespace Ashraak.Cctv.Amc.Infrastructure.Services;

internal sealed class AmcContractLookupService(
    IAmcContractRepository contractRepository,
    IAmcPlanRepository planRepository) : IAmcContractLookupService
{
    public async Task<AmcContractDetailDto?> GetByIdAsync(
        Guid contractId,
        CancellationToken cancellationToken = default)
    {
        var contract = await contractRepository.GetByIdAsync(AmcContractId.From(contractId), cancellationToken);
        if (contract is null)
            return null;

        var meta = await AmcPlanVersionMetaHelper.BuildAsync(contract, planRepository, cancellationToken);
        return AmcMapper.ToContractDetail(contract, meta);
    }

    public async Task<bool> HasActiveContractForSiteAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        var contract = await contractRepository.GetActiveBySiteIdAsync(siteId, cancellationToken);
        return contract is not null;
    }

    public async Task<AmcContractDetailDto?> GetActiveContractForSiteAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        var contract = await contractRepository.GetActiveBySiteIdAsync(siteId, cancellationToken);
        if (contract is null)
            return null;

        var meta = await AmcPlanVersionMetaHelper.BuildAsync(contract, planRepository, cancellationToken);
        return AmcMapper.ToContractDetail(contract, meta);
    }

    public async Task<AmcContractTermDetailDto?> GetTermByIdAsync(
        Guid contractId,
        Guid termId,
        CancellationToken cancellationToken = default)
    {
        var contract = await contractRepository.GetByIdAsync(AmcContractId.From(contractId), cancellationToken);
        if (contract is null)
            return null;

        var term = contract.Terms.FirstOrDefault(t => t.Id.Value == termId);
        if (term is null)
            return null;

        var version = await planRepository.GetVersionByIdAsync(term.PlanVersionId, cancellationToken);
        if (version is null)
            return null;

        var plan = await planRepository.GetByIdAsync(version.PlanId, cancellationToken);
        if (plan is null)
            return null;

        return AmcMapper.ToTermDetail(term, plan, version);
    }
}
