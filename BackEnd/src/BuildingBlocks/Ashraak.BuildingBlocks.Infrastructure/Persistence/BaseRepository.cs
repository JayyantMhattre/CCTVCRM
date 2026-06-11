using Ashraak.SharedKernel.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Generic EF Core repository base for aggregate roots.
/// Provides standard CRUD operations so module repositories only need to
/// add query-specific read methods (typically via Dapper for performance).
/// </summary>
/// <typeparam name="TAggregate">The aggregate root type managed by this repository.</typeparam>
/// <typeparam name="TId">The aggregate's identifier type (e.g. <c>TenantId</c>, <c>AuthUserId</c>).</typeparam>
/// <remarks>
/// The write side uses EF Core's change tracking for correctness.
/// The read/query side (list endpoints, reporting) should use Dapper directly
/// against a read-optimised connection to avoid loading unneeded navigation properties.
/// </remarks>
public abstract class BaseRepository<TAggregate, TId>(DbContext context)
    where TAggregate : AggregateRoot<TId>
    where TId : notnull
{
    /// <summary>The EF Core DbContext injected for this repository.</summary>
    protected readonly DbContext Context = context;

    /// <summary>The typed DbSet giving access to the aggregate's table.</summary>
    protected readonly DbSet<TAggregate> DbSet = context.Set<TAggregate>();

    /// <summary>
    /// Returns the aggregate with the given <paramref name="id"/>, or <see langword="null"/>.
    /// Uses EF Core's <c>FindAsync</c> which checks the identity map before querying the database.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    public virtual async Task<TAggregate?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        => await DbSet.FindAsync([id], cancellationToken);

    /// <summary>
    /// Adds a new aggregate to the change tracker for insertion on the next <c>SaveChangesAsync</c>.
    /// </summary>
    /// <param name="aggregate">The aggregate to add.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    public virtual async Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        => await DbSet.AddAsync(aggregate, cancellationToken);

    /// <summary>
    /// Marks an existing aggregate as modified for update on the next <c>SaveChangesAsync</c>.
    /// </summary>
    /// <param name="aggregate">The aggregate to update.</param>
    public virtual void Update(TAggregate aggregate)
        => DbSet.Update(aggregate);

    /// <summary>
    /// Marks an aggregate for deletion on the next <c>SaveChangesAsync</c>.
    /// Prefer soft-delete in the domain (e.g. <see cref="AggregateRoot{TId}"/> state flag)
    /// for data retention; use this only for hard-delete scenarios.
    /// </summary>
    /// <param name="aggregate">The aggregate to remove.</param>
    public virtual void Remove(TAggregate aggregate)
        => DbSet.Remove(aggregate);
}
