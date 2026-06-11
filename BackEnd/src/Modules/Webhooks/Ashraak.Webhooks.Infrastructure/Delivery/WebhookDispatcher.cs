using Ashraak.SharedKernel.Contracts.Webhooks.Events;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Ashraak.Webhooks.Infrastructure.Delivery;

internal sealed class WebhookDispatcher(
    IWebhookSubscriptionStore subscriptionStore,
    IWebhookDeliveryStore deliveryStore,
    IWebhookPayloadBuilder payloadBuilder,
    IWebhookDeliveryQueue deliveryQueue,
    IUnitOfWork unitOfWork,
    ILogger<WebhookDispatcher> logger) : IWebhookDispatcher
{
    public async Task DispatchAsync(WebhookRequestedEvent requestedEvent, CancellationToken cancellationToken)
    {
        var subscriptions = await subscriptionStore.GetEnabledForEventAsync(
            requestedEvent.TenantId,
            requestedEvent.EventName,
            cancellationToken);

        if (subscriptions.Count == 0)
        {
            logger.LogInformation(
                "No matching webhook subscriptions for {EventName} tenant {TenantId}",
                requestedEvent.EventName,
                requestedEvent.TenantId);
            return;
        }

        var envelope = payloadBuilder.BuildEnvelopeJson(new WebhookOutboundPayload(
            Guid.NewGuid(),
            requestedEvent.EventName,
            requestedEvent.Version,
            DateTime.UtcNow,
            requestedEvent.TenantId,
            requestedEvent.CorrelationId,
            requestedEvent.PayloadJson));

        foreach (var subscription in subscriptions)
        {
            var delivery = WebhookDelivery.CreatePending(
                WebhookDeliveryId.New(),
                subscription.Id.Value,
                subscription.TenantId,
                requestedEvent.EventName,
                requestedEvent.Version,
                requestedEvent.CorrelationId,
                envelope);

            deliveryStore.Add(delivery);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await deliveryQueue.EnqueueAsync(delivery.Id.Value, cancellationToken);
        }
    }
}
