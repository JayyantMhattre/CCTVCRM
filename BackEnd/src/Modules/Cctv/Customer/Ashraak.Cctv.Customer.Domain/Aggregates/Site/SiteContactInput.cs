namespace Ashraak.Cctv.Customer.Domain.Aggregates.Site;

/// <summary>Input for replacing the site contact set.</summary>
public sealed record SiteContactInput(
    string Name,
    string? Designation,
    string Phone,
    string? Email,
    bool IsPrimary);
