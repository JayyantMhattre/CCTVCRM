using Ashraak.SharedKernel.Interfaces;

namespace Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;

/// <summary>
/// Extends <see cref="IUnitOfWork"/> with explicit transaction management.
///
/// <para>
/// The base <see cref="IUnitOfWork.SaveChangesAsync"/> persists all staged
/// changes in a single implicit transaction (EF Core's default behaviour,
/// or a MongoDB session write).  Use the explicit transaction methods when
/// you need to coordinate writes across multiple operations (saga, two-step
/// create-then-publish, etc.).
/// </para>
///
/// <para>
/// <b>Typical pattern</b>:
/// <code>
/// await uow.BeginTransactionAsync(ct);
/// try
/// {
///     await repo.InsertAsync(order, ct);
///     await repo.UpdateAsync(inventory, ct);
///     await uow.SaveChangesAsync(ct);
///     await uow.CommitTransactionAsync(ct);
/// }
/// catch
/// {
///     await uow.RollbackTransactionAsync(ct);
///     throw;
/// }
/// </code>
/// </para>
/// </summary>
public interface IDataUnitOfWork : IUnitOfWork
{
    /// <summary>
    /// Opens a new database transaction.  Calling <see cref="IUnitOfWork.SaveChangesAsync"/>
    /// inside a transaction stages changes without committing until
    /// <see cref="CommitTransactionAsync"/> is called.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the active transaction and releases the transaction object.
    /// Throws <see cref="InvalidOperationException"/> when no transaction is active.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the active transaction and releases the transaction object.
    /// Safe to call even when no transaction is active (no-op in that case).
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
