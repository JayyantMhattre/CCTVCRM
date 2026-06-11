using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Tenant.Events;

/// <summary>
/// Raised when a tenant has been soft-deleted.
/// Handled by the Users module to deactivate all associated user profiles, and by
/// the Audit module to record the deletion. A background job will purge data after
/// the retention grace period expires.
/// </summary>
/// <param name="TenantId">The unique identifier of the deleted tenant.</param>
public sealed record TenantDeletedEvent(Guid TenantId) : DomainEvent;
