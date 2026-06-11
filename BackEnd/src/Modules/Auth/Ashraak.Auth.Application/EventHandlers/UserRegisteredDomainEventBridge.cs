using Ashraak.Auth.Domain.Aggregates.AuthUser.Events;
using Ashraak.SharedKernel.Contracts.Auth.Events;
using MediatR;

namespace Ashraak.Auth.Application.EventHandlers;

/// <summary>
/// Promotes <see cref="UserRegisteredDomainEvent"/> to the cross-module contract event.
/// </summary>
internal sealed class UserRegisteredDomainEventBridge : INotificationHandler<UserRegisteredDomainEvent>
{
    private readonly IPublisher _publisher;

    public UserRegisteredDomainEventBridge(IPublisher publisher) => _publisher = publisher;

    public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken) =>
        _publisher.Publish(
            new UserRegisteredEvent(
                notification.UserId,
                notification.TenantId,
                notification.Email,
                notification.DisplayName),
            cancellationToken);
}
