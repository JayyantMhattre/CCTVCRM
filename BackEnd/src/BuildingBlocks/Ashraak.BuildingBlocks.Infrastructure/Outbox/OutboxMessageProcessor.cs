using Ashraak.SharedKernel.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ashraak.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Shared outbox dispatch logic used by the hosted processor and optional Quartz jobs.
/// </summary>
public static class OutboxMessageProcessor
{
    public static async Task<int> ProcessBatchAsync<TDbContext>(
        TDbContext dbContext,
        IPublisher publisher,
        ILogger logger,
        int batchSize,
        CancellationToken cancellationToken)
        where TDbContext : DbContext
    {
        var messages = await dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.CreatedOnUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
            return 0;

        foreach (var message in messages)
        {
            try
            {
                var type = Type.GetType(message.Type);
                if (type is null)
                {
                    logger.LogWarning("Unknown outbox message type: {Type}", message.Type);
                    message.MarkAsFailed($"Unknown type: {message.Type}");
                    continue;
                }

                var payload = JsonSerializer.Deserialize(message.Content, type);
                if (payload is not INotification notification)
                {
                    message.MarkAsFailed("Payload is not a MediatR notification.");
                    continue;
                }

                await publisher.Publish(notification, cancellationToken);
                message.MarkAsProcessed();
            }
            catch (Exception ex)
            {
                message.MarkAsFailed(ex.Message);
                logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return messages.Count;
    }
}
