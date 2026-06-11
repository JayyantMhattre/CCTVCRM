namespace Ashraak.Auth.Domain.Aggregates.Invitation;

public enum InvitationStatus
{
    Pending = 0,
    Accepted = 1,
    Revoked = 2,
    Expired = 3
}
