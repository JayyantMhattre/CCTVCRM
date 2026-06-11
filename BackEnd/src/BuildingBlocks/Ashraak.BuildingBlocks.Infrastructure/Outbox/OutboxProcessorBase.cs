using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Ashraak.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Quartz job base that polls the outbox table. Prefer <see cref="OutboxProcessorHostedService{TDbContext}"/> in the host.
/// </summary>
[DisallowConcurrentExecution]
public abstract class OutboxProcessorBase<TDbContext>(
    TDbContext dbContext,
    IPublisher publisher,
    ILogger logger)
    : IJob
    where TDbContext : DbContext
{
    private const int BatchSize = 20;

    public async Task Execute(IJobExecutionContext context) =>
        await OutboxMessageProcessor.ProcessBatchAsync(
            dbContext,
            publisher,
            logger,
            BatchSize,
            context.CancellationToken);
}
