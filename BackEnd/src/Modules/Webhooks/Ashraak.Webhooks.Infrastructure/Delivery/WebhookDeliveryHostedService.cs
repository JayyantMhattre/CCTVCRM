using Ashraak.Webhooks.Application;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ashraak.Webhooks.Infrastructure.Delivery;

/// <summary>Processes queued webhook deliveries asynchronously (W2+W3).</summary>
internal sealed class WebhookDeliveryHostedService(
    WebhookDeliveryQueue deliveryQueue,
    IServiceScopeFactory scopeFactory,
    IOptions<WebhookDeliveryOptions> deliveryOptions,
    ILogger<WebhookDeliveryHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var maxConcurrent = Math.Max(1, deliveryOptions.Value.MaxConcurrentDeliveries);
        using var semaphore = new SemaphoreSlim(maxConcurrent, maxConcurrent);
        var workers = new List<Task>();

        await foreach (var deliveryId in deliveryQueue.ReadAllAsync(stoppingToken))
        {
            await semaphore.WaitAsync(stoppingToken);
            workers.Add(ProcessWithReleaseAsync(deliveryId, semaphore, stoppingToken));
            workers.RemoveAll(t => t.IsCompleted);
        }

        await Task.WhenAll(workers);
    }

    private async Task ProcessWithReleaseAsync(
        Guid deliveryId,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken)
    {
        try
        {
            await ProcessDeliveryAsync(deliveryId, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Unhandled error processing webhook delivery {DeliveryId}", deliveryId);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task ProcessDeliveryAsync(Guid deliveryId, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var deliveryStore = scope.ServiceProvider.GetRequiredService<IWebhookDeliveryStore>();
        var deliveryService = scope.ServiceProvider.GetRequiredService<IWebhookDeliveryService>();

        var delivery = await deliveryStore.GetByIdAsync(WebhookDeliveryId.From(deliveryId), cancellationToken);
        if (delivery is null)
        {
            logger.LogWarning("Webhook delivery {DeliveryId} not found for processing", deliveryId);
            return;
        }

        if (delivery.Status != Domain.Enums.WebhookDeliveryStatus.Pending)
            return;

        await deliveryService.ExecuteAsync(delivery, cancellationToken);
    }
}
