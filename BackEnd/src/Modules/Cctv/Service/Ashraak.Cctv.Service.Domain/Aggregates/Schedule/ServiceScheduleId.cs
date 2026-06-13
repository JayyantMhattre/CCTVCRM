namespace Ashraak.Cctv.Service.Domain.Aggregates.Schedule;

public readonly record struct ServiceScheduleId(Guid Value)
{
    public static ServiceScheduleId New() => new(Guid.NewGuid());
    public static ServiceScheduleId From(Guid value) => new(value);
}
