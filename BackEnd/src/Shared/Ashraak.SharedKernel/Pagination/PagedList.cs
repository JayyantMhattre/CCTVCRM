namespace Ashraak.SharedKernel.Pagination;

/// <summary>
/// Immutable, type-safe wrapper for a single page of query results.
/// Carries both the data items and the metadata needed by the client to
/// build pagination controls (total pages, next/previous flags).
/// </summary>
/// <typeparam name="T">The type of items in the current page.</typeparam>
/// <remarks>
/// <para>
/// Query handlers create instances via <see cref="Create"/> after executing a
/// Dapper or EF Core query with <c>OFFSET / FETCH</c> (or <c>Skip / Take</c>).
/// </para>
/// <para>
/// Use <see cref="Map{TTarget}"/> to project the items into a DTO type without
/// losing the pagination metadata.
/// </para>
/// </remarks>
public sealed class PagedList<T>
{
    private PagedList(IReadOnlyList<T> items, int page, int pageSize, long totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>Gets the items in the current page.</summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>Gets the 1-based current page number.</summary>
    public int Page { get; }

    /// <summary>Gets the maximum number of items per page.</summary>
    public int PageSize { get; }

    /// <summary>Gets the total number of items across all pages.</summary>
    public long TotalCount { get; }

    /// <summary>Gets the total number of pages given <see cref="TotalCount"/> and <see cref="PageSize"/>.</summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>Gets a value indicating whether there is a page after the current one.</summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>Gets a value indicating whether there is a page before the current one.</summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Creates a new <see cref="PagedList{T}"/> from an in-memory source.
    /// </summary>
    /// <param name="source">The items in the current page (already sliced, not the full data set).</param>
    /// <param name="page">1-based page number.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="totalCount">Total items across all pages (from a <c>COUNT</c> query).</param>
    public static PagedList<T> Create(IEnumerable<T> source, int page, int pageSize, long totalCount) =>
        new(source.ToList().AsReadOnly(), page, pageSize, totalCount);

    /// <summary>
    /// Creates an empty <see cref="PagedList{T}"/> preserving the requested page and size.
    /// Useful when a search returns no results.
    /// </summary>
    /// <param name="page">The requested page number.</param>
    /// <param name="pageSize">The requested page size.</param>
    public static PagedList<T> Empty(int page, int pageSize) =>
        new([], page, pageSize, 0);

    /// <summary>
    /// Projects every item through <paramref name="mapper"/> and returns a new paged list
    /// with the same pagination metadata.
    /// </summary>
    /// <typeparam name="TTarget">The target item type.</typeparam>
    /// <param name="mapper">Transform function applied to each item.</param>
    public PagedList<TTarget> Map<TTarget>(Func<T, TTarget> mapper) =>
        new(Items.Select(mapper).ToList().AsReadOnly(), Page, PageSize, TotalCount);
}
