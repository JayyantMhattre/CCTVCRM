namespace Ashraak.SharedKernel.Contracts.CctvCrm.Enums;

/// <summary>Service schedule status (serialized as PascalCase string).</summary>
public enum ScheduleStatusContract
{
    Planned,
    Assigned,
    InProgress,
    Completed,
    Missed,
    Cancelled
}
