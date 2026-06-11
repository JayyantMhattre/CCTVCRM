using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.BuildingBlocks.Data.Abstractions.Common;

/// <summary>
/// Base class for all data entities managed by the generic data layer.
/// Extends <see cref="Entity{Guid}"/> from SharedKernel to use a <see cref="Guid"/>
/// primary key, and adds standard audit/soft-delete fields.
///
/// <para>
/// <b>Usage</b>: Inherit from <see cref="BaseDataEntity"/> for any entity that should
/// be managed by <c>IDataRepository&lt;T&gt;</c>.
/// For DDD aggregate roots that need domain events use <c>AggregateRoot&lt;TId&gt;</c>
/// and <c>BaseRepository&lt;T, TId&gt;</c> from BuildingBlocks.Infrastructure instead.
/// </para>
///
/// <para>
/// <b>Multi-tenancy</b>: If the entity belongs to a specific tenant, inherit from
/// <see cref="TenantScopedEntity"/> instead — the repository will automatically
/// inject a <c>TenantId</c> filter on every query.
/// </para>
/// </summary>
public abstract class BaseDataEntity : Entity<Guid>
{
    /// <summary>Initialises a new entity with a freshly generated <see cref="Guid"/>.</summary>
    protected BaseDataEntity() : base(Guid.NewGuid()) { }

    /// <summary>Initialises a new entity with the supplied <paramref name="id"/>.</summary>
    protected BaseDataEntity(Guid id) : base(id) { }

    /// <summary>UTC timestamp when the entity was first persisted.</summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp of the last successful update.
    /// <see langword="null"/> when the entity has never been updated since creation.
    /// </summary>
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// When <see langword="true"/> the record is logically deleted and excluded from
    /// all default repository queries.  Use <c>SoftDeleteAsync</c> to set this flag.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// UTC timestamp of the soft-delete operation.
    /// <see langword="null"/> when the entity has not been soft-deleted.
    /// </summary>
    public DateTime? DeletedAtUtc { get; set; }
}
