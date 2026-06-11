namespace Ashraak.SharedKernel.Domain.Events;

/// <summary>
/// Abstraction for dispatching domain events to their handlers.
/// The default implementation wraps MediatR's <c>IPublisher</c>.
/// </summary>
/// <remarks>
/// In the normal write path, domain events are persisted via the outbox and
/// then dispatched by <c>OutboxProcessorBase</c>. This interface exists for
/// cases where immediate in-process dispatch is needed (e.g. projections that
/// must run in the same transaction).
/// </remarks>
public interface IDomainEventPublisher
{
    /// <summary>
    /// Publishes a single domain event to all registered handlers.
    /// </summary>
    /// <param name="domainEvent">The event to publish.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes multiple domain events to their handlers in order.
    /// </summary>
    /// <param name="domainEvents">The ordered sequence of events to publish.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
