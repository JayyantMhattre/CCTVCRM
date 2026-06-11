using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Auth.Events;

/// <summary>
/// Raised by the Auth module after a new identity is successfully registered.
/// Consumed by the Users module to create the corresponding profile/business record.
/// </summary>
/// <param name="UserId">Shared identifier of the new identity/profile pair.</param>
/// <param name="TenantId">Tenant that owns the new user.</param>
/// <param name="Email">Registered email address.</param>
/// <param name="DisplayName">Initial display name supplied at registration.</param>
public sealed record UserRegisteredEvent(
    Guid UserId,
    Guid TenantId,
    string Email,
    string DisplayName) : DomainEvent;
