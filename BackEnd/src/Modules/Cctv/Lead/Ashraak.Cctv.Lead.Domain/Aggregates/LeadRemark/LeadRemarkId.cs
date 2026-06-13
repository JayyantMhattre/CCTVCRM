namespace Ashraak.Cctv.Lead.Domain.Aggregates.LeadRemark;

public readonly record struct LeadRemarkId(Guid Value)
{
    public static LeadRemarkId New() => new(Guid.NewGuid());
    public static LeadRemarkId From(Guid value) => new(value);
}
