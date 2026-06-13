namespace Ashraak.Cctv.Service.Domain.Aggregates.Visit;

public sealed class VisitLocation
{
    private VisitLocation() { }

    public Guid Id { get; private set; }
    public ServiceVisitId ServiceVisitId { get; private set; }
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public DateTime CapturedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    public static VisitLocation Create(
        ServiceVisitId visitId,
        decimal latitude,
        decimal longitude,
        DateTime capturedAtUtc,
        Guid createdBy)
    {
        if (latitude is < -90m or > 90m)
            throw new InvalidOperationException("Latitude must be between -90 and 90.");

        if (longitude is < -180m or > 180m)
            throw new InvalidOperationException("Longitude must be between -180 and 180.");

        return new VisitLocation
        {
            Id = Guid.NewGuid(),
            ServiceVisitId = visitId,
            Latitude = latitude,
            Longitude = longitude,
            CapturedAtUtc = capturedAtUtc,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
