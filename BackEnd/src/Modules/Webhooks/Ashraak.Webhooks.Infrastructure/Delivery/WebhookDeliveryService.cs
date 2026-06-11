using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Application;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;
using Ashraak.Webhooks.Domain.Repositories;
using Ashraak.Webhooks.Infrastructure.Retry;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ashraak.Webhooks.Infrastructure.Delivery;

internal sealed class WebhookDeliveryService(
    IHttpClientFactory httpClientFactory,
    IWebhookSubscriptionStore subscriptionStore,
    IWebhookDeliveryStore deliveryStore,
    IWebhookSignatureService signatureService,
    IWebhookSecretProtector secretProtector,
    IWebhookDeliveryOutcomeHandler outcomeHandler,
    IUnitOfWork unitOfWork,
    IOptions<WebhookDeliveryOptions> deliveryOptions,
    WebhookDeliveryMetrics metrics,
    ILogger<WebhookDeliveryService> logger) : IWebhookDeliveryService
{
    public const string HttpClientName = "WebhookClient";

    public async Task ExecuteAsync(WebhookDelivery delivery, CancellationToken cancellationToken)
    {
        metrics.RecordAttempt();

        var subscription = await subscriptionStore.GetByIdAsync(
            WebhookSubscriptionId.From(delivery.SubscriptionId),
            cancellationToken);

        if (subscription is null || subscription.TenantId != delivery.TenantId)
        {
            await outcomeHandler.HandleFailureAsync(
                delivery, null, null, "Subscription not found or tenant mismatch.", cancellationToken);
            return;
        }

        if (!subscription.Enabled)
        {
            await outcomeHandler.HandleFailureAsync(
                delivery, null, null, "Subscription is disabled.", cancellationToken);
            return;
        }

        var options = deliveryOptions.Value;
        var secret = secretProtector.Unprotect(subscription.SecretProtected);
        var signature = signatureService.ComputeSignature(secret, delivery.Payload);

        using var request = new HttpRequestMessage(HttpMethod.Post, subscription.EndpointUrl)
        {
            Content = new StringContent(delivery.Payload, Encoding.UTF8, "application/json")
        };

        request.Headers.TryAddWithoutValidation("X-Webhook-Event", delivery.EventName);
        request.Headers.TryAddWithoutValidation("X-Webhook-Version", delivery.EventVersion);
        request.Headers.TryAddWithoutValidation("X-Webhook-Delivery-Id", delivery.Id.Value.ToString());
        if (!string.IsNullOrWhiteSpace(delivery.CorrelationId))
            request.Headers.TryAddWithoutValidation("X-Correlation-Id", delivery.CorrelationId);
        request.Headers.TryAddWithoutValidation("X-Webhook-Signature", signature);
        request.Headers.UserAgent.ParseAdd(options.UserAgent);

        var client = httpClientFactory.CreateClient(HttpClientName);
        client.Timeout = TimeSpan.FromSeconds(Math.Max(1, options.TimeoutSeconds));

        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var response = await client.SendAsync(request, cancellationToken);
            stopwatch.Stop();
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var statusCode = (int)response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                delivery.MarkSucceeded(statusCode, body);
                deliveryStore.Update(delivery);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                metrics.RecordSuccess(stopwatch.ElapsedMilliseconds);
                logger.LogInformation(
                    "Webhook delivery {DeliveryId} succeeded ({StatusCode}) attempt {AttemptNumber} in {DurationMs}ms correlation {CorrelationId}",
                    delivery.Id.Value,
                    statusCode,
                    delivery.AttemptNumber,
                    stopwatch.ElapsedMilliseconds,
                    delivery.CorrelationId);
            }
            else
            {
                await outcomeHandler.HandleFailureAsync(
                    delivery, statusCode, body, $"HTTP {statusCode}", cancellationToken);
                logger.LogWarning(
                    "Webhook delivery {DeliveryId} failed ({StatusCode}) attempt {AttemptNumber} in {DurationMs}ms correlation {CorrelationId}",
                    delivery.Id.Value,
                    statusCode,
                    delivery.AttemptNumber,
                    stopwatch.ElapsedMilliseconds,
                    delivery.CorrelationId);
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            stopwatch.Stop();
            await outcomeHandler.HandleFailureAsync(delivery, null, null, ex.Message, cancellationToken);
            logger.LogWarning(
                ex,
                "Webhook delivery {DeliveryId} transport error attempt {AttemptNumber} after {DurationMs}ms correlation {CorrelationId}",
                delivery.Id.Value,
                delivery.AttemptNumber,
                stopwatch.ElapsedMilliseconds,
                delivery.CorrelationId);
        }
    }
}
