using MediatR;

namespace Ashraak.SharedKernel.Domain.Events;

/// <summary>
/// Marker interface for all domain events in the system.
/// Domain events represent something that has already happened inside the domain
/// (past-tense: <c>TenantProvisioned</c>, <c>UserCreated</c>).
/// </summary>
/// <remarks>
/// <para>
/// Extends <see cref="INotification"/> so every domain event can be dispatched
/// by <c>IPublisher</c> (MediatR) without additional conversion.
/// </para>
/// <para>
/// Domain events are first serialised into the outbox table by <c>BaseDbContext</c>
/// and then replayed by <c>OutboxProcessorBase</c>, guaranteeing at-least-once delivery
/// even if the process crashes between the DB commit and the in-memory dispatch.
/// </para>
/// </remarks>
public interface IDomainEvent : INotification
{
    /// <summary>Gets the unique identifier for this event occurrence.</summary>
    Guid EventId { get; }

    /// <summary>Gets the UTC timestamp at which this event was raised.</summary>
    DateTime OccurredOnUtc { get; }
}
