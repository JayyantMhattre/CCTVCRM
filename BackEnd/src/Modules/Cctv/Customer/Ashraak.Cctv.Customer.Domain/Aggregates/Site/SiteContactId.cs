namespace Ashraak.Cctv.Customer.Domain.Aggregates.Site;

public readonly record struct SiteContactId(Guid Value)
{
    public static SiteContactId New() => new(Guid.NewGuid());

    public static SiteContactId From(Guid value) => new(value);
}
