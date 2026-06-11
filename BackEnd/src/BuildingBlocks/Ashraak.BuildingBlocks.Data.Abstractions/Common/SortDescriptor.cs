using System.Linq.Expressions;

namespace Ashraak.BuildingBlocks.Data.Abstractions.Common;

/// <summary>
/// Describes a single sort clause to apply on a repository query.
/// Pass a collection of <see cref="SortDescriptor{T}"/> to
/// <c>IReadRepository&lt;T&gt;.GetPagedAsync</c> for deterministic,
/// multi-column ordering.
/// </summary>
/// <typeparam name="T">The entity type being sorted.</typeparam>
public sealed class SortDescriptor<T>
{
    /// <summary>
    /// A lambda that selects the property to sort by,
    /// e.g. <c>x => x.CreatedAtUtc</c>.
    /// </summary>
    public required Expression<Func<T, object?>> KeySelector { get; init; }

    /// <summary>
    /// When <see langword="true"/> the sort order is descending (newest first).
    /// Defaults to <see langword="false"/> (ascending).
    /// </summary>
    public bool Descending { get; init; } = false;
}
