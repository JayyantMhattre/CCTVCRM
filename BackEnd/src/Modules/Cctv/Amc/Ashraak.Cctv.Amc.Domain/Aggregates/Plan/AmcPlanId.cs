namespace Ashraak.Cctv.Amc.Domain.Aggregates.Plan;

public readonly record struct AmcPlanId(Guid Value)
{
    public static AmcPlanId New() => new(Guid.NewGuid());
    public static AmcPlanId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
