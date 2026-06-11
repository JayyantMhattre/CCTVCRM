using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;

namespace Ashraak.Webhooks.Application.Abstractions;

public interface IWebhookDeliveryService
{
    Task ExecuteAsync(WebhookDelivery delivery, CancellationToken cancellationToken = default);
}
