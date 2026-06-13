using Ashraak.Cctv.Service.Domain.Enums;

namespace Ashraak.Cctv.Service.Domain.Aggregates.Visit;

public sealed class VisitPhoto
{
    private VisitPhoto() { }

    public Guid Id { get; private set; }
    public ServiceVisitId ServiceVisitId { get; private set; }
    public Guid FileId { get; private set; }
    public VisitPhotoCategory Category { get; private set; }
    public string? Caption { get; private set; }
    public DateTime CapturedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    public static VisitPhoto Create(
        ServiceVisitId visitId,
        Guid fileId,
        VisitPhotoCategory category,
        string? caption,
        DateTime capturedAtUtc,
        Guid createdBy)
    {
        return new VisitPhoto
        {
            Id = Guid.NewGuid(),
            ServiceVisitId = visitId,
            FileId = fileId,
            Category = category,
            Caption = caption?.Trim(),
            CapturedAtUtc = capturedAtUtc,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
