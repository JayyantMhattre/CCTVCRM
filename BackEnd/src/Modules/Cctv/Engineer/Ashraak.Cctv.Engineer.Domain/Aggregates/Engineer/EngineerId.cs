namespace Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer;

public readonly record struct EngineerId(Guid Value)
{
    public static EngineerId New() => new(Guid.NewGuid());
    public static EngineerId From(Guid value) => new(value);
}
