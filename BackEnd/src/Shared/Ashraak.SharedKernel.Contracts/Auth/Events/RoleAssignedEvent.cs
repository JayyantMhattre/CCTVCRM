using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Auth.Events;

/// <summary>
/// Raised when a role is granted to a user within a tenant.
/// Handled by the Audit module to maintain an RBAC change history.
/// Active sessions for the affected user should invalidate their permission cache.
/// </summary>
/// <param name="UserId">The user who received the new role.</param>
/// <param name="TenantId">The tenant context in which the role was assigned.</param>
/// <param name="RoleName">The name of the role that was assigned.</param>
/// <param name="AssignedByUserId">The administrator who performed the assignment.</param>
public sealed record RoleAssignedEvent(
    Guid UserId,
    Guid TenantId,
    string RoleName,
    Guid AssignedByUserId) : DomainEvent;
