namespace Ashraak.Cctv.Lead.Domain.Aggregates.LeadActivity;

public readonly record struct LeadActivityId(Guid Value)
{
    public static LeadActivityId New() => new(Guid.NewGuid());
    public static LeadActivityId From(Guid value) => new(value);
}
