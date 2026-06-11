namespace Ashraak.BuildingBlocks.EventBus;

/// <summary>
/// Base record for all integration events. Provides a stable envelope
/// that can be serialised into the outbox and replayed by any transport
/// (RabbitMQ via MassTransit, Azure Service Bus, etc.).
/// </summary>
public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    public string EventType => GetType().Name;
}
