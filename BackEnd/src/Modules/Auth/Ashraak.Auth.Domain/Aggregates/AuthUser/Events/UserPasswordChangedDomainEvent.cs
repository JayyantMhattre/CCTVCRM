using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Auth.Domain.Aggregates.AuthUser.Events;

/// <summary>
/// Raised by <see cref="AuthUser.ChangePassword"/> after a password has been changed or reset.
/// Triggers token revocation in the Auth module so all existing sessions become invalid,
/// and an audit log entry via the Audit module.
/// </summary>
/// <param name="UserId">The user whose password was changed.</param>
/// <param name="TenantId">The tenant context of the change.</param>
public sealed record UserPasswordChangedDomainEvent(Guid UserId, Guid TenantId) : DomainEvent;
