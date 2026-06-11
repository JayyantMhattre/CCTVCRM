using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Tenant.Domain.Aggregates.Tenant.Events;

/// <summary>
/// Raised by <see cref="Tenant.Delete"/> when the tenant has been soft-deleted.
/// Handled by the Users module to deactivate all <c>UserProfile</c> records for this tenant.
/// </summary>
/// <param name="TenantId">The identifier of the deleted tenant.</param>
public sealed record TenantDeletedDomainEvent(Guid TenantId) : DomainEvent;
