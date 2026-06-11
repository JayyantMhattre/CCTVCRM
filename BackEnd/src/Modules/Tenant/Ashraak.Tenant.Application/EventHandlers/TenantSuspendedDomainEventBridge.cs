using Ashraak.SharedKernel.Contracts.Tenant.Events;
using Ashraak.Tenant.Domain.Aggregates.Tenant.Events;
using MediatR;

namespace Ashraak.Tenant.Application.EventHandlers;

internal sealed class TenantSuspendedDomainEventBridge : INotificationHandler<TenantSuspendedDomainEvent>
{
    private readonly IPublisher _publisher;

    public TenantSuspendedDomainEventBridge(IPublisher publisher) => _publisher = publisher;

    public Task Handle(TenantSuspendedDomainEvent notification, CancellationToken cancellationToken) =>
        _publisher.Publish(
            new TenantSuspendedEvent(notification.TenantId, notification.Reason),
            cancellationToken);
}
