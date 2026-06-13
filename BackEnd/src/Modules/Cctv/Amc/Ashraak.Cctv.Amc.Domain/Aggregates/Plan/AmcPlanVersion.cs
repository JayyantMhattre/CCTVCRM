using Ashraak.Cctv.Amc.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Amc.Domain.Aggregates.Plan;

/// <summary>AMC plan version (composition under <see cref="AmcPlan"/>).</summary>
public sealed class AmcPlanVersion : Entity<AmcPlanVersionId>
{
    private AmcPlanVersion(AmcPlanVersionId id) : base(id) { }

    public AmcPlanId PlanId { get; private set; }
    public int VersionNo { get; private set; }
    public decimal Price { get; private set; }
    public int VisitFrequencyPerYear { get; private set; }
    public string IncludedServicesJson { get; private set; } = "[]";
    public string SlaTerms { get; private set; } = string.Empty;
    public DateOnly EffectiveFrom { get; private set; }
    public PlanVersionStatus Status { get; private set; }
    public bool IsReferenced { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    internal static AmcPlanVersion CreateDraft(
        AmcPlanVersionId id,
        AmcPlanId planId,
        int versionNo,
        decimal price,
        int visitFrequencyPerYear,
        string includedServicesJson,
        string slaTerms,
        DateOnly effectiveFrom,
        Guid createdBy)
    {
        if (price <= 0)
            throw new InvalidOperationException("Plan version price must be greater than zero.");

        if (visitFrequencyPerYear <= 0)
            throw new InvalidOperationException("Visit frequency must be greater than zero.");

        return new AmcPlanVersion(id)
        {
            PlanId = planId,
            VersionNo = versionNo,
            Price = price,
            VisitFrequencyPerYear = visitFrequencyPerYear,
            IncludedServicesJson = includedServicesJson,
            SlaTerms = slaTerms.Trim(),
            EffectiveFrom = effectiveFrom,
            Status = PlanVersionStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    internal void Publish()
    {
        if (Status != PlanVersionStatus.Draft)
            throw new InvalidOperationException("Only draft plan versions can be published.");

        Status = PlanVersionStatus.Published;
    }

    internal void Supersede()
    {
        if (Status != PlanVersionStatus.Published)
            throw new InvalidOperationException("Only published plan versions can be superseded.");

        Status = PlanVersionStatus.Superseded;
    }

    public void MarkReferenced()
    {
        IsReferenced = true;
    }

    internal void EnsureMutable()
    {
        if (Status != PlanVersionStatus.Draft)
            throw new InvalidOperationException("Published or superseded plan versions are immutable.");

        if (IsReferenced)
            throw new InvalidOperationException("Referenced plan versions cannot be modified.");
    }
}
