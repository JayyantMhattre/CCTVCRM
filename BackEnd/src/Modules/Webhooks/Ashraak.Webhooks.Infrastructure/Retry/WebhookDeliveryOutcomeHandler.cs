using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Application;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Enums;
using Ashraak.Webhooks.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ashraak.Webhooks.Infrastructure.Retry;

internal sealed class WebhookDeliveryOutcomeHandler(
    IWebhookFailureClassifier failureClassifier,
    IWebhookRetryBackoffCalculator backoffCalculator,
    IDeadLetterService deadLetterService,
    IWebhookDeliveryStore deliveryStore,
    IUnitOfWork unitOfWork,
    IOptions<WebhookRetryOptions> retryOptions,
    WebhookDeliveryMetrics metrics,
    ILogger<WebhookDeliveryOutcomeHandler> logger) : IWebhookDeliveryOutcomeHandler
{
    public async Task HandleFailureAsync(
        WebhookDelivery delivery,
        int? responseCode,
        string? responseBody,
        string? errorMessage,
        CancellationToken cancellationToken = default)
    {
        metrics.RecordFailure();
        var options = retryOptions.Value;

        if (!options.Enabled)
        {
            delivery.MarkFailed(responseCode, responseBody, errorMessage);
            deliveryStore.Update(delivery);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var failureType = failureClassifier.Classify(responseCode, errorMessage);

        if (failureType == WebhookFailureType.Permanent)
        {
            delivery.MarkFailed(responseCode, responseBody, errorMessage);
            deliveryStore.Update(delivery);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogWarning(
                "Webhook delivery {DeliveryId} permanent failure ({FailureCode}) correlation {CorrelationId}",
                delivery.Id.Value,
                responseCode,
                delivery.CorrelationId);
            return;
        }

        if (!backoffCalculator.CanRetry(delivery.AttemptNumber, options.MaxRetries))
        {
            await deadLetterService.MoveToDeadLetterAsync(
                delivery,
                responseCode,
                errorMessage ?? responseBody,
                cancellationToken);

            logger.LogWarning(
                "Webhook delivery {DeliveryId} moved to DLQ after {Attempts} attempts correlation {CorrelationId}",
                delivery.Id.Value,
                delivery.AttemptNumber,
                delivery.CorrelationId);
            return;
        }

        var delay = backoffCalculator.GetDelayBeforeNextAttempt(delivery.AttemptNumber);
        delivery.ScheduleRetry(DateTime.UtcNow.Add(delay), responseCode, errorMessage ?? responseBody);
        deliveryStore.Update(delivery);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        metrics.RecordRetryScheduled();

        logger.LogInformation(
            "Webhook delivery {DeliveryId} retry #{RetryCount} scheduled at {NextRetryOnUtc} correlation {CorrelationId}",
            delivery.Id.Value,
            delivery.RetryCount,
            delivery.NextRetryOnUtc,
            delivery.CorrelationId);
    }
}
