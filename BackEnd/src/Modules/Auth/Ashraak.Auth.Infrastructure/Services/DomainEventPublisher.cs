using Ashraak.SharedKernel.Domain.Events;
using MediatR;

namespace Ashraak.Auth.Infrastructure.Services;

/// <summary>
/// MediatR-backed implementation of <see cref="IDomainEventPublisher"/>.
/// Delegates to <see cref="IPublisher"/> which dispatches to all registered
/// <see cref="MediatR.INotificationHandler{TNotification}"/> instances in the DI container.
/// </summary>
/// <remarks>
/// Used for immediate in-process dispatch (e.g. after a command handler that needs
/// to chain domain event side effects synchronously). The normal path is through the
/// outbox: <c>BaseDbContext</c> serialises events → <c>OutboxProcessorBase</c> dispatches them.
/// </remarks>
internal sealed class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IPublisher _publisher;

    /// <summary>Initialises the publisher with the MediatR publisher.</summary>
    public DomainEventPublisher(IPublisher publisher) => _publisher = publisher;

    /// <inheritdoc/>
    public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default) =>
        await _publisher.Publish(domainEvent, cancellationToken);

    /// <inheritdoc/>
    /// <remarks>Events are published sequentially in the order provided.</remarks>
    public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
            await _publisher.Publish(domainEvent, cancellationToken);
    }
}
