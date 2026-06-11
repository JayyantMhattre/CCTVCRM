using Ashraak.SharedKernel.Contracts.Users.Events;
using Ashraak.Users.Domain.Aggregates.UserProfile.Events;
using MediatR;

namespace Ashraak.Users.Application.EventHandlers;

internal sealed class UserProfileCreatedDomainEventBridge : INotificationHandler<UserProfileCreatedDomainEvent>
{
    private readonly IPublisher _publisher;

    public UserProfileCreatedDomainEventBridge(IPublisher publisher) => _publisher = publisher;

    public Task Handle(UserProfileCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        _publisher.Publish(
            new UserCreatedEvent(
                notification.UserId,
                notification.TenantId,
                notification.Email,
                notification.DisplayName),
            cancellationToken);
}
