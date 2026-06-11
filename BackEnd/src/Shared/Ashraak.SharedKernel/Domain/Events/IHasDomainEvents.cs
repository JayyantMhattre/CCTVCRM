namespace Ashraak.SharedKernel.Domain.Events;

/// <summary>
/// Non-generic interface implemented by all aggregate roots.
/// Allows <c>BaseDbContext</c> to extract and clear pending domain events without
/// knowing the concrete identifier type (<c>TId</c>) of each aggregate.
/// </summary>
/// <remarks>
/// This is an infrastructure concern only — application and domain code should
/// not depend on this interface directly. Use the aggregate's own methods instead.
/// </remarks>
public interface IHasDomainEvents
{
    /// <summary>Gets the domain events that have been raised but not yet dispatched.</summary>
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Removes all pending domain events after they have been serialised into the outbox.
    /// </summary>
    void ClearDomainEvents();
}
