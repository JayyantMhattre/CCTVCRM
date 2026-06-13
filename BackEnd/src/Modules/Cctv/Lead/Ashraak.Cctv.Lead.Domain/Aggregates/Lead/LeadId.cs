namespace Ashraak.Cctv.Lead.Domain.Aggregates.Lead;

public readonly record struct LeadId(Guid Value)
{
    public static LeadId New() => new(Guid.NewGuid());
    public static LeadId From(Guid value) => new(value);
}
