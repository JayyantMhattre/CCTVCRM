namespace Ashraak.Cctv.Amc.Domain.Aggregates.Plan;

public readonly record struct AmcPlanVersionId(Guid Value)
{
    public static AmcPlanVersionId New() => new(Guid.NewGuid());
    public static AmcPlanVersionId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
