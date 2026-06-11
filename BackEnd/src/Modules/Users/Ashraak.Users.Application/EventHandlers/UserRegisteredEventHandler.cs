using Ashraak.SharedKernel.Contracts.Auth.Events;
using Ashraak.Users.Application.Commands.CreateUserProfile;
using MediatR;

namespace Ashraak.Users.Application.EventHandlers;

/// <summary>
/// Bridges Auth identity creation to Users profile creation.
/// Keeps module ownership clear: Auth creates identity, Users creates profile data.
/// </summary>
internal sealed class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly ISender _sender;

    /// <summary>Initialises the handler with MediatR sender.</summary>
    public UserRegisteredEventHandler(ISender sender)
    {
        _sender = sender;
    }

    /// <inheritdoc />
    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateUserProfileCommand(
                notification.UserId,
                notification.TenantId,
                notification.Email,
                notification.DisplayName),
            cancellationToken);

        // Bubble up failures so retry policies/outbox processors can re-attempt later.
        if (result.IsFailure)
            throw new InvalidOperationException(result.Error.Description);
    }
}
