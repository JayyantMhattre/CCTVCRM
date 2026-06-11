using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Auth.Events;

/// <summary>
/// Raised when all active tokens for a user are revoked.
/// Triggers a Redis cache invalidation so that subsequent requests with the old
/// token are rejected before reaching module handlers.
/// </summary>
/// <param name="UserId">The user whose tokens were revoked.</param>
/// <param name="TenantId">The tenant context in which revocation occurred.</param>
/// <param name="Reason">Human-readable reason (e.g. <c>"Password changed"</c>, <c>"Admin revocation"</c>).</param>
public sealed record TokenRevokedEvent(
    Guid UserId,
    Guid TenantId,
    string Reason) : DomainEvent;
