using System.Linq.Expressions;

namespace Ashraak.BuildingBlocks.Data.Abstractions.Common;

/// <summary>
/// Optional configuration bag passed to read methods that support
/// EF Core-specific features such as eager loading and change tracking.
///
/// <para>
/// Not all providers support every option:
/// <list type="bullet">
///   <item><see cref="Includes"/> — EF Core only (ignored by MongoDB).</item>
///   <item><see cref="TrackChanges"/> — EF Core only (MongoDB always returns detached documents).</item>
///   <item><see cref="SplitQuery"/> — EF Core only.</item>
/// </list>
/// </para>
/// </summary>
/// <typeparam name="T">The entity type being queried.</typeparam>
public sealed class QueryOptions<T> where T : class
{
    /// <summary>
    /// When <see langword="false"/> (default) the query uses <c>AsNoTracking()</c>
    /// — faster for read-only queries.  Set to <see langword="true"/> when you
    /// intend to update the returned entities in the same unit of work.
    /// </summary>
    public bool TrackChanges { get; init; } = false;

    /// <summary>
    /// Navigation properties to eagerly load via EF Core <c>Include</c>.
    /// Example: <c>new() { Includes = [x => x.Orders] }</c>.
    /// </summary>
    public List<Expression<Func<T, object?>>> Includes { get; init; } = [];

    /// <summary>
    /// When <see langword="true"/> EF Core will issue a separate SQL query per
    /// <c>Include</c> instead of a single JOIN.  Prevents the cartesian explosion
    /// problem for collections.  Only meaningful when <see cref="Includes"/> is
    /// non-empty.
    /// </summary>
    public bool SplitQuery { get; init; } = false;

    /// <summary>
    /// Default (read-only, no includes, no split query).
    /// </summary>
    public static readonly QueryOptions<T> Default = new();
}
