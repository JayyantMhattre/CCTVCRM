using Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Infrastructure;
using Ashraak.Webhooks.Infrastructure.Delivery;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Ashraak.Api.Health;

/// <summary>Verifies Webhooks module DI and delivery engine registration.</summary>
internal sealed class WebhooksHealthCheck(IServiceProvider services) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _ = services.GetRequiredService<IWebhookPublisher>();
            _ = services.GetRequiredService<IWebhookSubscriptionRepository>();
            _ = services.GetRequiredService<IWebhookDeliveryRepository>();
            _ = services.GetRequiredService<IWebhookDispatcher>();
            _ = services.GetRequiredService<IWebhookDeliveryService>();
            _ = services.GetRequiredService<IHttpClientFactory>().CreateClient(WebhooksModule.WebhookHttpClientName);

            var hosted = services.GetServices<IHostedService>();
            var hasDeliveryWorker = hosted.Any(s =>
                string.Equals(
                    s.GetType().FullName,
                    "Ashraak.Webhooks.Infrastructure.Delivery.WebhookDeliveryHostedService",
                    StringComparison.Ordinal));
            var hasRetryWorker = hosted.Any(s =>
                string.Equals(
                    s.GetType().FullName,
                    "Ashraak.Webhooks.Infrastructure.Retry.WebhookRetryHostedService",
                    StringComparison.Ordinal));

            if (!hasDeliveryWorker || !hasRetryWorker)
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    "Webhook delivery or retry hosted service is not registered."));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                "Webhooks module, delivery engine, and retry engine registered."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Webhooks delivery engine is not healthy.", ex));
        }
    }
}
