using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ashraak.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Background worker that polls a module outbox table and dispatches messages via MediatR.
/// </summary>
public sealed class OutboxProcessorHostedService<TDbContext>(
    IServiceScopeFactory scopeFactory,
    IOptions<OutboxProcessorOptions> options,
    ILogger<OutboxProcessorHostedService<TDbContext>> logger) : BackgroundService
    where TDbContext : DbContext
{
    private readonly OutboxProcessorOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Outbox processor started for {DbContext} (interval {Interval}s, batch {Batch})",
            typeof(TDbContext).Name,
            _options.PollInterval.TotalSeconds,
            _options.BatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
                var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

                var processed = await OutboxMessageProcessor.ProcessBatchAsync(
                    dbContext,
                    publisher,
                    logger,
                    _options.BatchSize,
                    stoppingToken);

                if (processed > 0)
                {
                    logger.LogDebug(
                        "Outbox processor {DbContext} dispatched {Count} message(s)",
                        typeof(TDbContext).Name,
                        processed);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox processor {DbContext} cycle failed", typeof(TDbContext).Name);
            }

            try
            {
                await Task.Delay(_options.PollInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        logger.LogInformation("Outbox processor stopped for {DbContext}", typeof(TDbContext).Name);
    }
}
