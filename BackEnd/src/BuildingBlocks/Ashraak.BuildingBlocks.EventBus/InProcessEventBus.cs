using Ashraak.SharedKernel.Outbox;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ashraak.BuildingBlocks.EventBus;

/// <summary>
/// Phase 1 implementation: serialises integration events into the shared outbox table.
/// The OutboxProcessor (Quartz job) picks them up and dispatches via MediatR internally.
/// Swap this registration for a MassTransit implementation when extracting to microservices.
/// </summary>
public sealed class InProcessEventBus(ILogger<InProcessEventBus> logger) : IEventBus
{
    public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        logger.LogInformation(
            "In-process event bus: {EventType} ({EventId})",
            integrationEvent.EventType,
            integrationEvent.EventId);

        // Phase 1: log only. Outbox processor will handle persistence.
        // Phase 3: replace body with MassTransit publish call.
        return Task.CompletedTask;
    }
}
