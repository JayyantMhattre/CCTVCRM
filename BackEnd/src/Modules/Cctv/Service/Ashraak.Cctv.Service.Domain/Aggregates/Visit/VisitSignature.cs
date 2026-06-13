namespace Ashraak.Cctv.Service.Domain.Aggregates.Visit;

public sealed class VisitSignature
{
    private VisitSignature() { }

    public Guid Id { get; private set; }
    public ServiceVisitId ServiceVisitId { get; private set; }
    public Guid FileId { get; private set; }
    public string SignedByName { get; private set; } = string.Empty;
    public DateTime CapturedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    public static VisitSignature Create(
        ServiceVisitId visitId,
        Guid fileId,
        string signedByName,
        DateTime capturedAtUtc,
        Guid createdBy)
    {
        if (string.IsNullOrWhiteSpace(signedByName))
            throw new InvalidOperationException("Signer name is required.");

        return new VisitSignature
        {
            Id = Guid.NewGuid(),
            ServiceVisitId = visitId,
            FileId = fileId,
            SignedByName = signedByName.Trim(),
            CapturedAtUtc = capturedAtUtc,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
