namespace Ashraak.SharedKernel.Outbox;

/// <summary>
/// EF Core entity that persists a domain event envelope in the outbox table.
/// Records are created by <c>BaseDbContext.SerializeDomainEventsToOutbox</c> in the
/// same transaction as the aggregate state change.
/// </summary>
/// <remarks>
/// The private constructor enforces creation through the <see cref="Create"/> factory,
/// preventing accidental instantiation with missing data. EF Core reconstructs existing
/// rows via the private parameterless constructor using reflection.
/// </remarks>
public sealed class OutboxMessage : IOutboxMessage
{
    /// <summary>Private constructor for EF Core materialisation.</summary>
    private OutboxMessage() { }

    /// <summary>
    /// Creates a new outbox record for the given domain event type and serialised content.
    /// </summary>
    /// <param name="type">
    /// Assembly-qualified type name of the domain event
    /// (use <c>domainEvent.GetType().AssemblyQualifiedName!</c>).
    /// </param>
    /// <param name="content">
    /// JSON-serialised domain event payload
    /// (use <c>JsonSerializer.Serialize(domainEvent, domainEvent.GetType())</c>).
    /// </param>
    /// <returns>A new, unpersisted <see cref="OutboxMessage"/> ready to be added to the DbContext.</returns>
    public static OutboxMessage Create(string type, string content) => new()
    {
        Id = Guid.NewGuid(),
        Type = type,
        Content = content,
        CreatedOnUtc = DateTime.UtcNow
    };

    /// <inheritdoc/>
    public Guid Id { get; private set; }

    /// <inheritdoc/>
    public string Type { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string Content { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public DateTime CreatedOnUtc { get; private set; }

    /// <inheritdoc/>
    public DateTime? ProcessedOnUtc { get; private set; }

    /// <inheritdoc/>
    public string? Error { get; private set; }

    /// <summary>
    /// Marks this message as successfully processed at the current UTC time.
    /// Called by <c>OutboxProcessorBase</c> after successful MediatR dispatch.
    /// </summary>
    public void MarkAsProcessed() => ProcessedOnUtc = DateTime.UtcNow;

    /// <summary>
    /// Records a processing failure with the given <paramref name="error"/> message.
    /// The message will be retried on the next processor run.
    /// </summary>
    /// <param name="error">Human-readable error or exception message.</param>
    public void MarkAsFailed(string error) => Error = error;
}
