using Ashraak.SharedKernel.Contracts.Tenant.Dtos;
using Ashraak.SharedKernel.Contracts.Tenant.Events;
using Ashraak.Tenant.Domain.Aggregates.Tenant.Events;
using MediatR;

namespace Ashraak.Tenant.Application.EventHandlers;

internal sealed class TenantCreatedDomainEventBridge : INotificationHandler<TenantCreatedDomainEvent>
{
    private readonly IPublisher _publisher;

    public TenantCreatedDomainEventBridge(IPublisher publisher) => _publisher = publisher;

    public Task Handle(TenantCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        _publisher.Publish(
            new TenantProvisionedEvent(
                notification.TenantId,
                notification.Name,
                notification.Slug,
                (TenantPlan)(int)notification.Plan,
                notification.OwnerUserId),
            cancellationToken);
}
