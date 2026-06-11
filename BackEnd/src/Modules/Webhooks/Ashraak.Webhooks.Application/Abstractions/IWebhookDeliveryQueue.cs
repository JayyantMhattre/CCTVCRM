namespace Ashraak.Webhooks.Application.Abstractions;

public interface IWebhookDeliveryQueue
{
    ValueTask EnqueueAsync(Guid deliveryId, CancellationToken cancellationToken = default);
}
