namespace Ashraak.SharedKernel.Outbox;

/// <summary>
/// Contract for a persisted outbox record.
/// The outbox pattern guarantees at-least-once delivery of domain events:
/// events are written to this table inside the same DB transaction as the aggregate change,
/// then asynchronously picked up by <c>OutboxProcessorBase</c> and dispatched via MediatR.
/// </summary>
/// <remarks>
/// This interface is implemented by <see cref="OutboxMessage"/> and exposed so that
/// infrastructure code (repositories, processors) can work without referencing the concrete type.
/// </remarks>
public interface IOutboxMessage
{
    /// <summary>Gets the unique identifier of this outbox record.</summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the assembly-qualified type name of the original domain event,
    /// e.g. <c>"Ashraak.Tenant.Domain.Aggregates.Tenant.Events.TenantProvisionedDomainEvent, Ashraak.Tenant.Domain"</c>.
    /// Used by <c>OutboxProcessorBase</c> to reconstruct the event via <see cref="Type.GetType(string)"/>.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Gets the JSON-serialised content of the domain event.
    /// Serialised with <c>System.Text.Json</c> using the concrete event type.
    /// </summary>
    string Content { get; }

    /// <summary>Gets the UTC timestamp at which this record was created and inserted.</summary>
    DateTime CreatedOnUtc { get; }

    /// <summary>
    /// Gets the UTC timestamp at which this record was successfully processed.
    /// <see langword="null"/> indicates the message has not yet been dispatched.
    /// </summary>
    DateTime? ProcessedOnUtc { get; }

    /// <summary>
    /// Gets the error message if processing failed.
    /// <see langword="null"/> when the record has not yet been attempted or succeeded.
    /// </summary>
    string? Error { get; }
}
