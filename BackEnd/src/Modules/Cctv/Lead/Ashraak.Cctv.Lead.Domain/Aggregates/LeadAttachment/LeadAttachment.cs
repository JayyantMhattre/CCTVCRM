using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Lead.Domain.Aggregates.LeadAttachment;

/// <summary>File attachment linked to a lead via platform FileId.</summary>
public sealed class LeadAttachment : Entity<LeadAttachmentId>
{
    private LeadAttachment(LeadAttachmentId id) : base(id) { }

    public Lead.LeadId LeadId { get; private set; }
    public Guid FileId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public bool IsDeleted { get; private set; }

    public static LeadAttachment Create(
        LeadAttachmentId id,
        Lead.LeadId leadId,
        Guid fileId,
        string title,
        Guid createdBy)
    {
        return new LeadAttachment(id)
        {
            LeadId = leadId,
            FileId = fileId,
            Title = title.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void SoftDelete(Guid deletedBy)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        _ = deletedBy;
    }
}
