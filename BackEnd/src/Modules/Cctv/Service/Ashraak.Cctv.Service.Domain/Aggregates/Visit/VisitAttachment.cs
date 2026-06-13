using Ashraak.Cctv.Service.Domain.Enums;

namespace Ashraak.Cctv.Service.Domain.Aggregates.Visit;

public sealed class VisitAttachment
{
    private VisitAttachment() { }

    public Guid Id { get; private set; }
    public ServiceVisitId ServiceVisitId { get; private set; }
    public Guid FileId { get; private set; }
    public VisitAttachmentType AttachmentType { get; private set; }
    public string? Title { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    public static VisitAttachment Create(
        ServiceVisitId visitId,
        Guid fileId,
        VisitAttachmentType attachmentType,
        string? title,
        Guid createdBy)
    {
        return new VisitAttachment
        {
            Id = Guid.NewGuid(),
            ServiceVisitId = visitId,
            FileId = fileId,
            AttachmentType = attachmentType,
            Title = title?.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
