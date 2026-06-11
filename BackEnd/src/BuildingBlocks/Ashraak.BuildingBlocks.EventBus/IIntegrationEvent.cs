namespace Ashraak.BuildingBlocks.EventBus;

/// <summary>
/// Marker interface for integration events — events that cross module or service boundaries.
/// These are published via IEventBus (MassTransit/RabbitMQ) rather than MediatR.
/// When a module is extracted into a microservice, its domain events are promoted to
/// integration events without touching business logic.
/// </summary>
public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredOn { get; }
    string EventType { get; }
}
