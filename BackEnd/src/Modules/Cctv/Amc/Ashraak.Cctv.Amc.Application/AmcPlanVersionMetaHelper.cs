using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Repositories;
using ContractAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Contract.AmcContract;

namespace Ashraak.Cctv.Amc.Application;

internal static class AmcPlanVersionMetaHelper
{
    public static async Task<Dictionary<Guid, (string PlanCode, int VersionNo)>> BuildAsync(
        ContractAggregate contract,
        IAmcPlanRepository planRepository,
        CancellationToken cancellationToken)
    {
        var meta = new Dictionary<Guid, (string PlanCode, int VersionNo)>();
        foreach (var term in contract.Terms)
        {
            if (meta.ContainsKey(term.PlanVersionId.Value))
                continue;

            var version = await planRepository.GetVersionByIdAsync(term.PlanVersionId, cancellationToken);
            if (version is null)
                continue;

            var plan = await planRepository.GetByIdAsync(version.PlanId, cancellationToken);
            if (plan is null)
                continue;

            meta[version.Id.Value] = (plan.PlanCode, version.VersionNo);
        }

        return meta;
    }
}
