using Ashraak.Auth.Domain.Aggregates.Invitation.Events;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Auth.Domain.Aggregates.Invitation;

public sealed class Invitation : AggregateRoot<InvitationId>
{
    private Invitation(InvitationId id) : base(id) { }

    public Guid TenantId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Role { get; private set; } = "Member";
    public string TokenHash { get; private set; } = string.Empty;
    public InvitationStatus Status { get; private set; }
    public DateTime ExpiresOnUtc { get; private set; }
    public Guid InvitedByUserId { get; private set; }
    public Guid? AcceptedByUserId { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime UpdatedOnUtc { get; private set; }

    public static Invitation Create(
        Guid tenantId,
        string email,
        string role,
        string tokenHash,
        DateTime expiresOnUtc,
        Guid invitedByUserId)
    {
        var invitation = new Invitation(InvitationId.New())
        {
            TenantId = tenantId,
            Email = email.ToLowerInvariant(),
            Role = role,
            TokenHash = tokenHash,
            Status = InvitationStatus.Pending,
            ExpiresOnUtc = expiresOnUtc,
            InvitedByUserId = invitedByUserId,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };

        invitation.RaiseDomainEvent(new InvitationCreatedDomainEvent(
            invitation.Id.Value,
            tenantId,
            invitation.Email,
            role,
            invitedByUserId,
            expiresOnUtc));

        return invitation;
    }

    public bool IsExpired => DateTime.UtcNow > ExpiresOnUtc;

    public void Resend(string newTokenHash, DateTime newExpiresOnUtc, Guid resentByUserId)
    {
        if (Status != InvitationStatus.Pending)
            throw new InvalidOperationException("Only pending invitations can be resent.");

        TokenHash = newTokenHash;
        ExpiresOnUtc = newExpiresOnUtc;
        UpdatedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new InvitationResentDomainEvent(
            Id.Value, TenantId, Email, resentByUserId, newExpiresOnUtc));
    }

    public void Revoke(Guid revokedByUserId)
    {
        if (Status == InvitationStatus.Accepted)
            throw new InvalidOperationException("Accepted invitations cannot be revoked.");

        Status = InvitationStatus.Revoked;
        UpdatedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new InvitationRevokedDomainEvent(
            Id.Value, TenantId, Email, revokedByUserId));
    }

    public void Accept(Guid acceptedByUserId)
    {
        if (Status != InvitationStatus.Pending)
            throw new InvalidOperationException("Invitation is not pending.");

        if (IsExpired)
        {
            Status = InvitationStatus.Expired;
            throw new InvalidOperationException("Invitation has expired.");
        }

        Status = InvitationStatus.Accepted;
        AcceptedByUserId = acceptedByUserId;
        UpdatedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new InvitationAcceptedDomainEvent(
            Id.Value, TenantId, Email, acceptedByUserId));
    }
}
