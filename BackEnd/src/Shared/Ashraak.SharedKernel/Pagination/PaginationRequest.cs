namespace Ashraak.SharedKernel.Pagination;

/// <summary>
/// Encapsulates all pagination, sorting, and search parameters sent by an API consumer.
/// Bound from query-string parameters in Minimal API endpoints and passed directly
/// to the corresponding query object.
/// </summary>
/// <remarks>
/// Always call <see cref="WithClampedPageSize"/> before passing to a repository to prevent
/// clients from requesting arbitrarily large result sets that would stress the database.
/// </remarks>
public sealed record PaginationRequest
{
    /// <summary>The default page number (first page).</summary>
    public const int DefaultPage = 1;

    /// <summary>The default number of items per page.</summary>
    public const int DefaultPageSize = 20;

    /// <summary>
    /// The maximum allowed page size. Requests exceeding this are clamped to prevent
    /// accidentally loading entire tables.
    /// </summary>
    public const int MaxPageSize = 100;

    /// <summary>Gets or initialises the requested 1-based page number. Defaults to <see cref="DefaultPage"/>.</summary>
    public int Page { get; init; } = DefaultPage;

    /// <summary>Gets or initialises the number of items per page. Defaults to <see cref="DefaultPageSize"/>.</summary>
    public int PageSize { get; init; } = DefaultPageSize;

    /// <summary>Gets or initialises the property name to sort by (e.g. <c>"CreatedAt"</c>). <see langword="null"/> uses the default order.</summary>
    public string? SortBy { get; init; }

    /// <summary>Gets or initialises a value indicating whether to sort in descending order.</summary>
    public bool SortDescending { get; init; }

    /// <summary>Gets or initialises a free-text search term applied by the repository.</summary>
    public string? Search { get; init; }

    /// <summary>
    /// Gets the zero-based offset to pass to <c>OFFSET</c> / <c>Skip()</c>.
    /// Derived from <see cref="Page"/> and <see cref="PageSize"/>.
    /// </summary>
    public int Skip => (Page - 1) * PageSize;

    /// <summary>
    /// Returns a copy of this request with <see cref="PageSize"/> clamped to
    /// <c>[1, <see cref="MaxPageSize"/>]</c> and <see cref="Page"/> floored to 1.
    /// Always call this in repository methods before executing queries.
    /// </summary>
    public PaginationRequest WithClampedPageSize() => this with
    {
        PageSize = Math.Clamp(PageSize, 1, MaxPageSize),
        Page = Math.Max(Page, 1)
    };
}
