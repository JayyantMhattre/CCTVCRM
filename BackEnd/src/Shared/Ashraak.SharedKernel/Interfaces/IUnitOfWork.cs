namespace Ashraak.SharedKernel.Interfaces;

/// <summary>
/// Represents a unit of work that can persist changes atomically to the underlying store.
/// Implemented by each module's <c>DbContext</c> (which inherits from <c>BaseDbContext</c>).
/// </summary>
/// <remarks>
/// <para>
/// Application-layer command handlers depend on this interface rather than on a concrete
/// <c>DbContext</c> type, keeping the application layer free of infrastructure concerns.
/// </para>
/// <para>
/// <see cref="SaveChangesAsync"/> also triggers domain event serialisation to the outbox
/// table via <c>BaseDbContext.SerializeDomainEventsToOutbox</c>.
/// </para>
/// </remarks>
public interface IUnitOfWork
{
    /// <summary>
    /// Persists all pending changes in the current unit of work to the database.
    /// Domain events raised by aggregates are serialised to the outbox table as part of
    /// the same transaction.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    /// <returns>The number of database rows affected.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
