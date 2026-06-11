namespace Ashraak.SharedKernel.Domain.Primitives;

/// <summary>
/// Base class for all domain entities.
/// Entity identity is defined by its <typeparamref name="TId"/> value, not its reference.
/// Two entities are equal when they share the same concrete type and the same identifier,
/// regardless of where they live in memory.
/// </summary>
/// <typeparam name="TId">
/// The type of the entity identifier (e.g. <see cref="Guid"/>, a strongly-typed Id record).
/// Must be non-null.
/// </typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    /// <summary>
    /// Initialises a new entity with the given identifier.
    /// </summary>
    /// <param name="id">The unique identifier. Must not be the default value for its type.</param>
    protected Entity(TId id)
    {
        Id = id;
    }

    /// <summary>Gets the unique identifier of this entity.</summary>
    public TId Id { get; private init; }

    /// <summary>
    /// Returns <see langword="true"/> when both operands are non-null and represent
    /// the same domain entity (same type + same Id).
    /// </summary>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        left is not null && right is not null && left.Equals(right);

    /// <summary>
    /// Returns <see langword="true"/> when the operands are considered unequal.
    /// </summary>
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);

    /// <inheritdoc/>
    public bool Equals(Entity<TId>? other) =>
        other is not null && GetType() == other.GetType() && Id.Equals(other.Id);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Entity<TId> entity && Equals(entity);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
