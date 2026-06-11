using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Application;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ashraak.Webhooks.Infrastructure.Retry;

/// <summary>Scans retrying deliveries and schedules re-execution when due (W3).</summary>
internal sealed class WebhookRetryHostedService(
    IServiceScopeFactory scopeFactory,
    IOptions<WebhookRetryOptions> retryOptions,
    ILogger<WebhookRetryHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pollInterval = TimeSpan.FromSeconds(Math.Max(5, retryOptions.Value.PollIntervalSeconds));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (retryOptions.Value.Enabled)
                    await ProcessDueRetriesAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error processing webhook retry batch");
            }

            await Task.Delay(pollInterval, stoppingToken);
        }
    }

    private async Task ProcessDueRetriesAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var deliveryStore = scope.ServiceProvider.GetRequiredService<IWebhookDeliveryStore>();
        var deliveryQueue = scope.ServiceProvider.GetRequiredService<IWebhookDeliveryQueue>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var due = await deliveryStore.GetDueForRetryAsync(DateTime.UtcNow, batchSize: 50, cancellationToken);
        if (due.Count == 0)
            return;

        foreach (var delivery in due)
        {
            delivery.BeginRetryAttempt();
            deliveryStore.Update(delivery);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var delivery in due)
            await deliveryQueue.EnqueueAsync(delivery.Id.Value, cancellationToken);

        logger.LogInformation("Scheduled {Count} webhook delivery retries", due.Count);
    }
}
