using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;

namespace Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;

/// <summary>
/// Platform entry point for modules to publish webhook-eligible events.
/// W1: enqueues to outbox only — no HTTP delivery.
/// </summary>
public interface IWebhookPublisher
{
    /// <summary>
    /// Validates the event against the catalog and enqueues a <see cref="Events.WebhookRequestedEvent"/>.
    /// </summary>
    Task PublishAsync(WebhookEventContract webhookEvent, CancellationToken cancellationToken = default);
}
