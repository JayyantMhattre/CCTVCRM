using Ashraak.Cctv.Lead.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Lead.Domain.Aggregates.LeadActivity;

/// <summary>Append-only activity log for a lead.</summary>
public sealed class LeadActivity : Entity<LeadActivityId>
{
    private LeadActivity(LeadActivityId id) : base(id) { }

    public Lead.LeadId LeadId { get; private set; }
    public LeadActivityType ActivityType { get; private set; }
    public LeadStatus? FromStatus { get; private set; }
    public LeadStatus? ToStatus { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateTime OccurredAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    public static LeadActivity Create(
        LeadActivityId id,
        Lead.LeadId leadId,
        LeadActivityType activityType,
        string description,
        LeadStatus? fromStatus,
        LeadStatus? toStatus,
        Guid createdBy)
    {
        var now = DateTime.UtcNow;
        return new LeadActivity(id)
        {
            LeadId = leadId,
            ActivityType = activityType,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            Description = description.Trim(),
            OccurredAtUtc = now,
            CreatedAtUtc = now,
            CreatedBy = createdBy
        };
    }

    public static LeadActivity CreateStatusChange(
        LeadActivityId id,
        Lead.LeadId leadId,
        LeadStatus fromStatus,
        LeadStatus toStatus,
        string description,
        Guid createdBy) =>
        Create(
            id,
            leadId,
            LeadActivityType.StatusChange,
            description,
            fromStatus,
            toStatus,
            createdBy);
}
