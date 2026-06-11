using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Auth.Domain.Aggregates.Invitation.Events;

public sealed record InvitationCreatedDomainEvent(
    Guid InvitationId,
    Guid TenantId,
    string Email,
    string Role,
    Guid InvitedByUserId,
    DateTime ExpiresOnUtc) : DomainEvent;

public sealed record InvitationResentDomainEvent(
    Guid InvitationId,
    Guid TenantId,
    string Email,
    Guid InvitedByUserId,
    DateTime ExpiresOnUtc) : DomainEvent;

public sealed record InvitationRevokedDomainEvent(
    Guid InvitationId,
    Guid TenantId,
    string Email,
    Guid RevokedByUserId) : DomainEvent;

public sealed record InvitationAcceptedDomainEvent(
    Guid InvitationId,
    Guid TenantId,
    string Email,
    Guid AcceptedByUserId) : DomainEvent;
