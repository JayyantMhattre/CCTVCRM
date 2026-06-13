using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Cctv.Customer.Domain.Aggregates.Site.Events;

public sealed record SiteCreatedDomainEvent(
    Guid SiteId,
    string SiteNumber,
    Guid CustomerId,
    string Name) : DomainEvent;

public sealed record SiteUpdatedDomainEvent(
    Guid SiteId,
    string SiteNumber,
    Guid UpdatedBy) : DomainEvent;

public sealed record SiteDeactivatedDomainEvent(
    Guid SiteId,
    string SiteNumber,
    Guid DeactivatedBy) : DomainEvent;

public sealed record SiteContactChangedDomainEvent(
    Guid SiteId,
    string SiteNumber,
    int ContactCount,
    Guid ChangedBy) : DomainEvent;

public sealed record SiteAssetSummaryUpdatedDomainEvent(
    Guid SiteId,
    string SiteNumber,
    Guid UpdatedBy) : DomainEvent;
