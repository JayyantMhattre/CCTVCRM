using Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;
using MongoDB.Driver;

namespace Ashraak.BuildingBlocks.Data.Mongo;

/// <summary>
/// MongoDB implementation of <see cref="IDataUnitOfWork"/>.
///
/// <para>
/// MongoDB documents are written immediately (no change-tracker).
/// The <see cref="SaveChangesAsync"/> method is therefore a no-op — it exists
/// only for interface compatibility with command handlers that call
/// <c>uow.SaveChangesAsync()</c> after every operation.
/// </para>
///
/// <para>
/// <b>Transactions</b>: MongoDB multi-document transactions require a replica set
/// or sharded cluster.  <see cref="BeginTransactionAsync"/> opens a client session
/// and starts a transaction.  All subsequent write operations in
/// <see cref="MongoDataRepository{T}"/> use the same session automatically via
/// <see cref="Session"/>.
/// </para>
///
/// <para>
/// Register as <c>Scoped</c> and inject both <see cref="MongoDataUnitOfWork"/>
/// and the matching repositories in the same DI scope.
/// </para>
/// </summary>
public sealed class MongoDataUnitOfWork : IDataUnitOfWork, IAsyncDisposable
{
    private readonly IMongoClient _client;
    private IClientSessionHandle? _session;
    private bool _transactionActive;

    public MongoDataUnitOfWork(IMongoClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <summary>
    /// The active <see cref="IClientSessionHandle"/>.
    /// <see langword="null"/> when no transaction has been started.
    /// Passed to every collection operation by <see cref="MongoDataRepository{T}"/>
    /// (the driver accepts a <see langword="null"/> session and works without
    /// a transaction in that case).
    /// </summary>
    public IClientSessionHandle? Session => _session;

    // ── IUnitOfWork ──────────────────────────────────────────────────────────

    /// <summary>
    /// No-op for MongoDB — documents are already persisted by each write call.
    /// Returns 0 (no rows affected concept in document stores).
    /// </summary>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(0);

    // ── IDataUnitOfWork ──────────────────────────────────────────────────────

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when a transaction is already active.</exception>
    /// <exception cref="NotSupportedException">
    /// MongoDB multi-document transactions require a replica set or sharded cluster.
    /// This will throw a server-side error when used against a standalone MongoDB.
    /// </exception>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transactionActive)
            throw new InvalidOperationException(
                "A MongoDB transaction is already active.  " +
                "Commit or rollback before starting a new one.");

        _session = await _client.StartSessionAsync(cancellationToken: cancellationToken);
        _session.StartTransaction();
        _transactionActive = true;
    }

    /// <inheritdoc/>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session is null || !_transactionActive)
            throw new InvalidOperationException("No active MongoDB transaction to commit.");

        try
        {
            await _session.CommitTransactionAsync(cancellationToken);
        }
        finally
        {
            _transactionActive = false;
        }
    }

    /// <inheritdoc/>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session is null || !_transactionActive) return; // safe no-op

        try
        {
            await _session.AbortTransactionAsync(cancellationToken);
        }
        finally
        {
            _transactionActive = false;
        }
    }

    // ── IAsyncDisposable ────────────────────────────────────────────────────

    public async ValueTask DisposeAsync()
    {
        if (_transactionActive && _session is not null)
        {
            // Abort any uncommitted transaction on disposal to avoid phantom writes.
            await RollbackTransactionAsync();
        }

        _session?.Dispose();
        _session = null;
    }
}
