namespace Ashraak.SharedKernel.Domain.Events;

/// <summary>
/// Abstract base record for concrete domain events.
/// Provides a stable <see cref="EventId"/> and <see cref="OccurredOnUtc"/> timestamp
/// so every event is naturally identifiable and chronologically ordered.
/// </summary>
/// <remarks>
/// Use <c>record</c> inheritance so that structural equality and positional construction
/// work out of the box for event types:
/// <code>
/// public sealed record TenantProvisionedDomainEvent(TenantId TenantId, string Name)
///     : DomainEvent;
/// </code>
/// </remarks>
public abstract record DomainEvent : IDomainEvent
{
    /// <summary>
    /// Initialises the event with a new unique identifier and the current UTC time.
    /// </summary>
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
    }

    /// <inheritdoc/>
    public Guid EventId { get; }

    /// <inheritdoc/>
    public DateTime OccurredOnUtc { get; }
}
