using Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ashraak.BuildingBlocks.Data.Sql;

/// <summary>
/// EF Core implementation of <see cref="IDataUnitOfWork"/>.
///
/// <para>
/// Wraps a single <see cref="DbContext"/> instance and exposes:
/// <list type="bullet">
///   <item><see cref="SaveChangesAsync"/> — flushes all pending EF Core change-tracker
///   entries to the database (within the active transaction when one is open).</item>
///   <item><see cref="BeginTransactionAsync"/> — opens a real database transaction so
///   multiple <c>SaveChangesAsync</c> calls can be atomically committed or rolled back.</item>
/// </list>
/// </para>
///
/// <para>
/// Register as <c>Scoped</c>: one instance per HTTP request, sharing the same
/// <see cref="DbContext"/> as the <see cref="GenericSqlRepository{T}"/> in that request.
/// </para>
/// </summary>
public sealed class SqlDataUnitOfWork : IDataUnitOfWork
{
    private readonly DbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public SqlDataUnitOfWork(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // ── IUnitOfWork ──────────────────────────────────────────────────────────

    /// <summary>
    /// Saves all pending changes tracked by the <see cref="DbContext"/>.
    /// When inside a transaction, changes are staged but not committed until
    /// <see cref="CommitTransactionAsync"/> is called.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    // ── IDataUnitOfWork ──────────────────────────────────────────────────────

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a transaction is already active.
    /// </exception>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
            throw new InvalidOperationException(
                "A transaction is already active.  Commit or rollback the current " +
                "transaction before starting a new one.");

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when no transaction is active.</exception>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException("No active transaction to commit.");

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            // On any failure during commit, roll back to keep the DB consistent.
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    /// <inheritdoc/>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null) return; // safe no-op

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    // ── IDisposable ──────────────────────────────────────────────────────────

    private async ValueTask DisposeTransactionAsync()
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
}
