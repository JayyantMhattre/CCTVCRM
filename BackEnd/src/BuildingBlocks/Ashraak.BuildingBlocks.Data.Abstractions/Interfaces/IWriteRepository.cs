using System.Linq.Expressions;
using Ashraak.BuildingBlocks.Data.Abstractions.Common;

namespace Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;

/// <summary>
/// Write-side contract for mutating <typeparamref name="T"/> entities.
///
/// <para>
/// All write operations are staged through the active
/// <c>IDataUnitOfWork</c> / <c>DbContext</c> and only persisted when
/// <c>SaveChangesAsync</c> is called.  MongoDB documents are written
/// immediately (session-based transaction optional via <c>IDataUnitOfWork</c>).
/// </para>
///
/// <para>
/// Multi-tenancy: <c>InsertAsync</c> / <c>InsertBulkAsync</c> automatically
/// stamp the current tenant's <c>TenantId</c> on entities that implement
/// <see cref="ITenantScoped"/>.
/// </para>
/// </summary>
public interface IWriteRepository<T> where T : BaseDataEntity
{
    // ── Insert ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Stages <paramref name="entity"/> for insertion.
    /// <c>CreatedAtUtc</c> is set automatically; <c>TenantId</c> is stamped from
    /// the active <c>ITenantContext</c> when the entity is <see cref="ITenantScoped"/>.
    /// </summary>
    Task InsertAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages all <paramref name="entities"/> for bulk insertion.
    /// EF Core uses <c>AddRangeAsync</c>; MongoDB uses <c>InsertManyAsync</c>.
    /// </summary>
    Task InsertBulkAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    // ── Update ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Marks the entire <paramref name="entity"/> as modified.
    /// <c>UpdatedAtUtc</c> is set automatically.
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads all entities matching <paramref name="filter"/>, applies
    /// <paramref name="updater"/>, and stages them for update.
    ///
    /// <para>
    /// <b>Performance note</b>: This performs a SELECT-then-UPDATE for each matching
    /// record (N+1 round-trips).  For high-volume batch updates use
    /// <c>IAdvancedRepository.ExecuteRawSqlAsync</c> with a targeted UPDATE statement.
    /// </para>
    /// </summary>
    /// <returns>The number of entities updated.</returns>
    Task<int> UpdateByFilterAsync(
        Expression<Func<T, bool>> filter,
        Action<T> updater,
        CancellationToken cancellationToken = default);

    // ── Delete ───────────────────────────────────────────────────────────────

    /// <summary>Hard-deletes the <paramref name="entity"/> from the store.</summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hard-deletes all entities matching <paramref name="filter"/>.
    /// EF Core uses <c>ExecuteDeleteAsync</c> (single SQL DELETE); MongoDB uses
    /// <c>DeleteManyAsync</c>.
    /// </summary>
    /// <returns>The number of entities deleted.</returns>
    Task<int> DeleteByFilterAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets <c>IsDeleted = true</c> and <c>DeletedAtUtc = UtcNow</c> on the entity
    /// with the given <paramref name="id"/>.  The record remains in the database
    /// but is excluded from all future default queries.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> when the entity was found and soft-deleted;
    /// <see langword="false"/> when no entity with that <paramref name="id"/> exists
    /// or the entity is already deleted.
    /// </returns>
    Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
