using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Enums;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

namespace Ashraak.Cctv.Amc.Infrastructure.Services;

internal sealed class AmcPlanLookupService(IAmcPlanRepository repository) : IAmcPlanLookupService
{
    public async Task<AmcPlanVersionDetailDto?> GetPublishedVersionAsync(
        Guid planVersionId,
        CancellationToken cancellationToken = default)
    {
        var version = await repository.GetVersionByIdAsync(AmcPlanVersionId.From(planVersionId), cancellationToken);
        if (version is null || version.Status != PlanVersionStatus.Published)
            return null;

        var plan = await repository.GetByIdAsync(version.PlanId, cancellationToken);
        return plan is null ? null : AmcMapper.ToPlanVersionDetail(plan, version);
    }

    public async Task<bool> IsPlanVersionPublishedAsync(
        Guid planVersionId,
        CancellationToken cancellationToken = default)
    {
        var version = await repository.GetVersionByIdAsync(AmcPlanVersionId.From(planVersionId), cancellationToken);
        return version is not null && version.Status == PlanVersionStatus.Published;
    }
}
