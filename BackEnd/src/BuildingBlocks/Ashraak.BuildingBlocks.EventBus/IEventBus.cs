namespace Ashraak.BuildingBlocks.EventBus;

/// <summary>
/// Abstraction for publishing integration events across module or service boundaries.
/// </summary>
/// <remarks>
/// <para>
/// Phase 1 (current): implemented by <see cref="InProcessEventBus"/> which logs the event
/// and relies on the outbox processor for persistence. No external broker is involved.
/// </para>
/// <para>
/// Phase 3 (microservice extraction): replace the registration with a MassTransit
/// implementation that publishes to RabbitMQ or Azure Service Bus. No code changes
/// required in callers because they depend on this interface.
/// </para>
/// </remarks>
public interface IEventBus
{
    /// <summary>
    /// Publishes an integration event to the configured transport.
    /// </summary>
    /// <typeparam name="TEvent">The integration event type.</typeparam>
    /// <param name="integrationEvent">The event to publish.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}
