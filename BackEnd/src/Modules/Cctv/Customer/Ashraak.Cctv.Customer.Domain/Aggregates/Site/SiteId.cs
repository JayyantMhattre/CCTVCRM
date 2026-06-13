namespace Ashraak.Cctv.Customer.Domain.Aggregates.Site;

public readonly record struct SiteId(Guid Value)
{
    public static SiteId New() => new(Guid.NewGuid());

    public static SiteId From(Guid value) => new(value);
}
