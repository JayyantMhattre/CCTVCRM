using Ashraak.Cctv.Amc.Domain.Aggregates.Plan.Events;
using Ashraak.Cctv.Amc.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Amc.Domain.Aggregates.Plan;

/// <summary>AMC plan aggregate root (schema <c>cctv_amc.amc_plans</c>).</summary>
public sealed class AmcPlan : AggregateRoot<AmcPlanId>
{
    private readonly List<AmcPlanVersion> _versions = [];

    private AmcPlan(AmcPlanId id) : base(id) { }

    public string PlanCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public PlanStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public uint RowVersion { get; private set; }

    public IReadOnlyList<AmcPlanVersion> Versions => _versions.AsReadOnly();

    public static AmcPlan Create(
        AmcPlanId id,
        string planCode,
        string name,
        string? description,
        Guid createdBy)
    {
        if (string.IsNullOrWhiteSpace(planCode))
            throw new ArgumentException("Plan code is required.", nameof(planCode));

        var plan = new AmcPlan(id)
        {
            PlanCode = planCode.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Description = NormalizeOptional(description),
            Status = PlanStatus.Active,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy,
            RowVersion = 1
        };

        plan.RaiseDomainEvent(new PlanCreatedDomainEvent(id.Value, plan.PlanCode, plan.Name, createdBy));
        return plan;
    }

    public void UpdateIdentity(string name, string? description, Guid updatedBy)
    {
        EnsureActive();

        Name = name.Trim();
        Description = NormalizeOptional(description);
        Touch(updatedBy);
    }

    public void Retire(Guid retiredBy)
    {
        if (Status == PlanStatus.Retired)
            return;

        Status = PlanStatus.Retired;
        Touch(retiredBy);
        RaiseDomainEvent(new PlanRetiredDomainEvent(Id.Value, PlanCode, retiredBy));
    }

    public AmcPlanVersion AddVersion(
        AmcPlanVersionId versionId,
        decimal price,
        int visitFrequencyPerYear,
        string includedServicesJson,
        string slaTerms,
        DateOnly effectiveFrom,
        Guid createdBy)
    {
        EnsureActive();

        var versionNo = _versions.Count == 0 ? 1 : _versions.Max(v => v.VersionNo) + 1;
        var version = AmcPlanVersion.CreateDraft(
            versionId,
            Id,
            versionNo,
            price,
            visitFrequencyPerYear,
            includedServicesJson,
            slaTerms,
            effectiveFrom,
            createdBy);

        _versions.Add(version);
        Touch(createdBy);
        return version;
    }

    public void PublishVersion(AmcPlanVersionId versionId, Guid publishedBy)
    {
        EnsureActive();

        var version = GetVersion(versionId);
        version.Publish();

        foreach (var other in _versions.Where(v => v.Id != versionId && v.Status == PlanVersionStatus.Published))
            other.Supersede();

        Touch(publishedBy);
        RaiseDomainEvent(new PlanVersionPublishedDomainEvent(
            Id.Value, versionId.Value, version.VersionNo, publishedBy));
    }

    public AmcPlanVersion GetVersion(AmcPlanVersionId versionId) =>
        _versions.FirstOrDefault(v => v.Id == versionId)
        ?? throw new InvalidOperationException("Plan version not found.");

    private void EnsureActive()
    {
        if (Status == PlanStatus.Retired)
            throw new InvalidOperationException("Retired plans are read-only.");
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private void Touch(Guid userId)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = userId;
        RowVersion++;
    }
}
