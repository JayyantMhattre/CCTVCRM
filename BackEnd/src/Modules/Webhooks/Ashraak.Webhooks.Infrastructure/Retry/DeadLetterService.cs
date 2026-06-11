using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Application;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDeadLetter;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace Ashraak.Webhooks.Infrastructure.Retry;

internal sealed class DeadLetterService(
    IWebhookDeadLetterStore deadLetterStore,
    IWebhookDeliveryStore deliveryStore,
    IWebhookDeliveryQueue deliveryQueue,
    IUnitOfWork unitOfWork,
    IOptions<WebhookDLQOptions> dlqOptions,
    WebhookDeliveryMetrics metrics) : IDeadLetterService
{
    public async Task MoveToDeadLetterAsync(
        WebhookDelivery delivery,
        int? failureCode,
        string? failureReason,
        CancellationToken cancellationToken = default)
    {
        if (!dlqOptions.Value.Enabled)
        {
            delivery.MarkFailed(failureCode, null, failureReason);
            deliveryStore.Update(delivery);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var deadLetter = WebhookDeadLetter.Create(
            WebhookDeadLetterId.New(),
            delivery.Id.Value,
            delivery.SubscriptionId,
            delivery.TenantId,
            delivery.EventName,
            delivery.Payload,
            failureReason,
            failureCode,
            delivery.RetryCount,
            delivery.CorrelationId);

        delivery.MarkDeadLettered(failureCode, failureReason);
        deadLetterStore.Add(deadLetter);
        deliveryStore.Update(delivery);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        metrics.RecordDeadLetterCreated();
    }

    public async Task<WebhookDelivery> ReplayAsync(
        WebhookDeadLetterId deadLetterId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var deadLetter = await deadLetterStore.GetByIdAsync(deadLetterId, cancellationToken)
            ?? throw new InvalidOperationException("Webhook dead letter was not found.");

        if (deadLetter.TenantId != tenantId)
            throw new InvalidOperationException("Webhook dead letter was not found.");

        var eventVersion = ExtractEventVersion(deadLetter.Payload);

        var newDelivery = WebhookDelivery.CreatePending(
            WebhookDeliveryId.New(),
            deadLetter.SubscriptionId,
            deadLetter.TenantId,
            deadLetter.EventName,
            eventVersion,
            deadLetter.CorrelationId,
            deadLetter.Payload);

        deadLetter.MarkReplayed(newDelivery.Id.Value);
        deliveryStore.Add(newDelivery);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await deliveryQueue.EnqueueAsync(newDelivery.Id.Value, cancellationToken);
        metrics.RecordDeadLetterReplayed();

        return newDelivery;
    }

    private static string ExtractEventVersion(string payload)
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(payload);
            if (doc.RootElement.TryGetProperty("version", out var version))
                return version.GetString() ?? "v1";
        }
        catch (System.Text.Json.JsonException)
        {
            // fall through
        }

        return "v1";
    }
}
