using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Domain.Primitives;

/// <summary>
/// Base class for DDD aggregate roots.
/// An aggregate root is the single entry point into an aggregate cluster of entities.
/// It owns a private list of <see cref="IDomainEvent"/>s that are raised during state
/// transitions and dispatched via MediatR after the current <c>SaveChangesAsync</c> call
/// (through <see cref="BaseDbContext"/>).
/// </summary>
/// <typeparam name="TId">
/// The type of the aggregate root identifier. Usually a strongly-typed Id record
/// (e.g. <c>TenantId</c>, <c>UserId</c>).
/// </typeparam>
/// <remarks>
/// Domain events are not persisted directly — they are serialised into the outbox table
/// by <c>BaseDbContext.SerializeDomainEventsToOutbox</c> so they survive process crashes.
/// </remarks>
public abstract class AggregateRoot<TId> : Entity<TId>, IHasDomainEvents
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Initialises a new aggregate root with the given identifier.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    protected AggregateRoot(TId id) : base(id) { }

    /// <summary>
    /// Gets all domain events that have been raised by this aggregate since the last
    /// <see cref="ClearDomainEvents"/> call.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Appends a domain event to the internal event list.
    /// Call this from aggregate methods that change state, e.g.
    /// <c>RaiseDomainEvent(new TenantProvisionedDomainEvent(this));</c>
    /// </summary>
    /// <param name="domainEvent">The event to raise.</param>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    /// <summary>
    /// Removes all pending domain events. Called by <c>BaseDbContext</c> after events
    /// have been serialised into the outbox table.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
