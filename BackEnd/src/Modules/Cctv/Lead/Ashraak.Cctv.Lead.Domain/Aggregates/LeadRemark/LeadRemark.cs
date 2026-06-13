using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Lead.Domain.Aggregates.LeadRemark;

/// <summary>Append-only remark on a lead.</summary>
public sealed class LeadRemark : Entity<LeadRemarkId>
{
    private LeadRemark(LeadRemarkId id) : base(id) { }

    public Lead.LeadId LeadId { get; private set; }
    public string Remark { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    public static LeadRemark Create(LeadRemarkId id, Lead.LeadId leadId, string text, Guid createdBy)
    {
        return new LeadRemark(id)
        {
            LeadId = leadId,
            Remark = text.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
