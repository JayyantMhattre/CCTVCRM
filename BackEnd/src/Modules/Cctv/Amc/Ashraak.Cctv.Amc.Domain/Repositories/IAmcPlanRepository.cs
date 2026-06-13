using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Enums;
using PlanAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Plan.AmcPlan;

namespace Ashraak.Cctv.Amc.Domain.Repositories;

public sealed record AmcPlanListResult(IReadOnlyList<PlanAggregate> Items, long TotalCount);

public interface IAmcPlanRepository
{
    Task<PlanAggregate?> GetByIdAsync(AmcPlanId id, CancellationToken cancellationToken);

    Task<PlanAggregate?> GetByCodeAsync(string planCode, CancellationToken cancellationToken);

    Task<AmcPlanVersion?> GetVersionByIdAsync(AmcPlanVersionId id, CancellationToken cancellationToken);

    Task<bool> IsVersionReferencedAsync(AmcPlanVersionId id, CancellationToken cancellationToken);

    Task<AmcPlanListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        PlanStatus? status,
        string? search,
        CancellationToken cancellationToken);

    void Add(PlanAggregate plan);
}
