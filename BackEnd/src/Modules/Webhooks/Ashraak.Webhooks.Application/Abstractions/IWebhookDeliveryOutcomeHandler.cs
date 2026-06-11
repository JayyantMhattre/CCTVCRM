using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;

namespace Ashraak.Webhooks.Application.Abstractions;

public interface IWebhookDeliveryOutcomeHandler
{
    Task HandleFailureAsync(
        WebhookDelivery delivery,
        int? responseCode,
        string? responseBody,
        string? errorMessage,
        CancellationToken cancellationToken = default);
}
