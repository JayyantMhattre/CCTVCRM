using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Auth.Domain.Aggregates.AuthUser.Events;

public sealed record FailedLoginDomainEvent(
    Guid? UserId,
    Guid TenantId,
    string Email,
    string IpAddress,
    string UserAgent) : DomainEvent;

public sealed record InvalidPasswordDomainEvent(
    Guid UserId,
    Guid TenantId,
    string Email,
    string IpAddress,
    string UserAgent) : DomainEvent;

public sealed record AccountLockedDomainEvent(
    Guid UserId,
    Guid TenantId,
    string Email,
    DateTime LockedUntilUtc) : DomainEvent;

public sealed record AccountUnlockedDomainEvent(
    Guid UserId,
    Guid TenantId,
    string Email) : DomainEvent;

public sealed record PasswordResetRequestedDomainEvent(
    Guid UserId,
    Guid TenantId,
    string Email,
    string ResetToken) : DomainEvent;

public sealed record MfaEnabledDomainEvent(
    Guid UserId,
    Guid TenantId,
    string Email) : DomainEvent;

public sealed record MfaDisabledDomainEvent(
    Guid UserId,
    Guid TenantId,
    string Email) : DomainEvent;

public sealed record MfaChallengeFailedDomainEvent(
    Guid UserId,
    Guid TenantId,
    string Email,
    string IpAddress) : DomainEvent;

public sealed record MfaVerifiedDomainEvent(
    Guid UserId,
    Guid TenantId,
    string Email) : DomainEvent;

public sealed record TokenRevokedDomainEvent(
    Guid UserId,
    Guid TenantId,
    string TokenId) : DomainEvent;

public sealed record RevokeAllSessionsDomainEvent(
    Guid UserId,
    Guid TenantId) : DomainEvent;
