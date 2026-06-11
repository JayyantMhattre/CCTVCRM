namespace Ashraak.BuildingBlocks.Data.Abstractions.Common;

/// <summary>
/// Immutable wrapper for a paginated slice of data returned by
/// <c>IReadRepository&lt;T&gt;.GetPagedAsync</c>.
/// </summary>
/// <typeparam name="T">The element type of the page.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>The items on the current page.</summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>The 1-based page number that was requested.</summary>
    public int PageNumber { get; }

    /// <summary>The maximum number of items per page that was requested.</summary>
    public int PageSize { get; }

    /// <summary>Total number of records across all pages (after applying filters).</summary>
    public long TotalCount { get; }

    /// <summary>Total number of pages calculated from <see cref="TotalCount"/> / <see cref="PageSize"/>.</summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary><see langword="true"/> when a previous page exists.</summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary><see langword="true"/> when a next page exists.</summary>
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedResult(IReadOnlyList<T> items, int pageNumber, int pageSize, long totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// Creates an empty result for the given pagination parameters.
    /// Useful as a safe default when no data matches the filter.
    /// </summary>
    public static PagedResult<T> Empty(int pageNumber, int pageSize) =>
        new([], pageNumber, pageSize, 0);
}
