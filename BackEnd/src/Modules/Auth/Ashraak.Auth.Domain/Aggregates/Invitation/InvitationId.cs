namespace Ashraak.Auth.Domain.Aggregates.Invitation;

public sealed record InvitationId(Guid Value)
{
    public static InvitationId New() => new(Guid.NewGuid());
    public static InvitationId From(Guid value) => new(value);
}
