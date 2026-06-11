using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Auth.Domain.Aggregates.AuthUser.Events;

/// <summary>Raised when a new <see cref="AuthUser"/> is registered.</summary>
public sealed record UserRegisteredDomainEvent(
    Guid UserId,
    Guid TenantId,
    string Email,
    string DisplayName) : DomainEvent;
