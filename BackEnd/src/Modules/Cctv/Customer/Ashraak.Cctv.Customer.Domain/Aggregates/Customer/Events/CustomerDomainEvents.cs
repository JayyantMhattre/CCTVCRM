using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Cctv.Customer.Domain.Aggregates.Customer.Events;

/// <summary>Raised when a new customer record is created.</summary>
public sealed record CustomerCreatedDomainEvent(
    Guid CustomerId,
    string CustomerNumber,
    string Name,
    string Email,
    Guid? SourceLeadId) : DomainEvent;

/// <summary>Raised when customer details are updated.</summary>
public sealed record CustomerUpdatedDomainEvent(
    Guid CustomerId,
    string CustomerNumber,
    Guid UpdatedBy) : DomainEvent;

/// <summary>Raised when a customer is deactivated.</summary>
public sealed record CustomerDeactivatedDomainEvent(
    Guid CustomerId,
    string CustomerNumber,
    Guid DeactivatedBy) : DomainEvent;
