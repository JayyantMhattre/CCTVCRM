using System.Linq.Expressions;
using Ashraak.BuildingBlocks.Data.Abstractions.Common;

namespace Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;

/// <summary>
/// Read-only contract for querying <typeparamref name="T"/> entities.
///
/// <para>
/// All methods automatically exclude soft-deleted records (<c>IsDeleted == false</c>)
/// and, for <see cref="ITenantScoped"/> entities, restrict to the current tenant.
/// </para>
///
/// <para>
/// <b>Provider notes</b>:
/// <list type="bullet">
///   <item><c>Query()</c> returns <see cref="IQueryable{T}"/> — EF Core only.</item>
///   <item>MongoDB does not support <c>Query()</c>; use <c>GetByFilterAsync</c> instead.</item>
/// </list>
/// </para>
/// </summary>
public interface IReadRepository<T> where T : BaseDataEntity
{
    // ── Basic lookups ────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the entity with the given <paramref name="id"/>,
    /// or <see langword="null"/> when not found or soft-deleted.
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all non-deleted entities in the collection.
    /// Use with care on large tables — prefer <see cref="GetPagedAsync"/> for collections.
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    // ── Filter-based queries ─────────────────────────────────────────────────

    /// <summary>
    /// Returns all entities matching <paramref name="filter"/> (excluding soft-deleted).
    /// </summary>
    Task<IReadOnlyList<T>> GetByFilterAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities matching <paramref name="filter"/> with optional
    /// EF Core-specific <paramref name="options"/> (includes, tracking, split query).
    /// </summary>
    Task<IReadOnlyList<T>> GetByFilterAsync(
        Expression<Func<T, bool>> filter,
        QueryOptions<T> options,
        CancellationToken cancellationToken = default);

    // ── Pagination ───────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a single page of results.
    /// </summary>
    /// <param name="pageNumber">1-based page index.</param>
    /// <param name="pageSize">Maximum items per page.</param>
    /// <param name="filter">Optional additional predicate (on top of IsDeleted / TenantId).</param>
    /// <param name="sortDescriptors">Ordered list of sort clauses.  When empty, results are in natural database order.</param>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        IEnumerable<SortDescriptor<T>>? sortDescriptors = null,
        CancellationToken cancellationToken = default);

    // ── Existence / count ────────────────────────────────────────────────────

    /// <summary>Returns <see langword="true"/> when at least one non-deleted entity matches <paramref name="filter"/>.</summary>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the number of non-deleted entities matching the optional <paramref name="filter"/>.
    /// Counts all when <paramref name="filter"/> is <see langword="null"/>.
    /// </summary>
    Task<long> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);

    // ── IQueryable escape hatch (EF Core only) ───────────────────────────────

    /// <summary>
    /// Exposes the base-filtered <see cref="IQueryable{T}"/> for complex scenarios
    /// (e.g. projections, GROUP BY).  The base filters (IsDeleted, TenantId) are
    /// <b>already applied</b> — do not re-apply them.
    ///
    /// <para>
    /// <b>Warning</b>: This method is EF Core only.  MongoDB implementations throw
    /// <see cref="NotSupportedException"/>.  Use only in SQL modules where you have
    /// confirmed the provider.
    /// </para>
    /// </summary>
    /// <param name="tracking">
    /// <see langword="true"/> to enable change tracking (use when you intend to update
    /// the returned entities).  Defaults to <see langword="false"/> for read performance.
    /// </param>
    IQueryable<T> Query(bool tracking = false);
}
