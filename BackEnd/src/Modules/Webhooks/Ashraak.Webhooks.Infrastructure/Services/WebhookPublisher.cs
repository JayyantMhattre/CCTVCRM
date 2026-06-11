using Ashraak.BuildingBlocks.Infrastructure.Outbox;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Contracts.Webhooks;
using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Contracts.Webhooks.Events;
using Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Domain.Repositories;

namespace Ashraak.Webhooks.Infrastructure.Services;

internal sealed class WebhookPublisher(
    IWebhookEventDefinitionStore eventDefinitions,
    IFeatureFlagService featureFlags,
    IOutboxWriter outboxWriter,
    IUnitOfWork unitOfWork) : IWebhookPublisher
{
    public async Task PublishAsync(WebhookEventContract webhookEvent, CancellationToken cancellationToken = default)
    {
        if (!await featureFlags.IsEnabledAsync(WebhookFeatureFlags.Enabled, webhookEvent.TenantId, cancellationToken))
            throw new InvalidOperationException("Webhooks are not enabled for this tenant.");

        if (string.IsNullOrWhiteSpace(webhookEvent.EventName))
            throw new ArgumentException("Event name is required.", nameof(webhookEvent));

        if (string.IsNullOrWhiteSpace(webhookEvent.PayloadJson))
            throw new ArgumentException("Payload is required.", nameof(webhookEvent));

        var definition = await eventDefinitions.GetByEventNameAsync(webhookEvent.EventName, cancellationToken);
        if (definition is null || !definition.Enabled)
            throw new InvalidOperationException($"Webhook event '{webhookEvent.EventName}' is not registered or is disabled.");

        var version = string.IsNullOrWhiteSpace(webhookEvent.Version) ? definition.Version : webhookEvent.Version;

        outboxWriter.Enqueue(new WebhookRequestedEvent(
            webhookEvent.TenantId,
            definition.EventName,
            version,
            webhookEvent.PayloadJson,
            webhookEvent.CorrelationId));

        outboxWriter.Enqueue(new WebhookPublishedDomainEvent(
            webhookEvent.TenantId,
            definition.EventName,
            version,
            webhookEvent.CorrelationId));

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
