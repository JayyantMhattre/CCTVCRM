namespace Ashraak.Cctv.Service.Domain.Aggregates.Schedule;

public sealed class EngineerAssignment
{
    private EngineerAssignment() { }

    public Guid Id { get; private set; }
    public ServiceScheduleId ServiceScheduleId { get; private set; }
    public Guid EngineerId { get; private set; }
    public Guid AssignedBy { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    public static EngineerAssignment Create(
        ServiceScheduleId scheduleId,
        Guid engineerId,
        Guid assignedBy)
    {
        return new EngineerAssignment
        {
            Id = Guid.NewGuid(),
            ServiceScheduleId = scheduleId,
            EngineerId = engineerId,
            AssignedBy = assignedBy,
            AssignedAtUtc = DateTime.UtcNow,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = assignedBy
        };
    }

    internal void Deactivate() => IsActive = false;
}
