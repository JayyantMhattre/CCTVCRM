namespace Ashraak.SharedKernel.Domain.Primitives;

/// <summary>
/// Base class for DDD value objects.
/// Value objects have no identity — two value objects are considered equal
/// when all of their constituent properties are equal.
/// </summary>
/// <remarks>
/// <para>
/// Derived classes must implement <see cref="GetAtomicValues"/> and yield every
/// property that participates in equality. Order matters:
/// <code>
/// protected override IEnumerable&lt;object?&gt; GetAtomicValues()
/// {
///     yield return Street;
///     yield return City;
///     yield return PostalCode;
/// }
/// </code>
/// </para>
/// <para>
/// Value objects should be immutable. Prefer <c>init</c>-only setters or
/// constructor-only initialisation to prevent accidental mutation.
/// </para>
/// </remarks>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Returns the ordered sequence of values that make up this value object's identity.
    /// Every property that should be compared must be yielded here.
    /// </summary>
    protected abstract IEnumerable<object?> GetAtomicValues();

    /// <inheritdoc/>
    public bool Equals(ValueObject? other) =>
        other is not null && GetType() == other.GetType() && GetAtomicValues().SequenceEqual(other.GetAtomicValues());

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ValueObject other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var value in GetAtomicValues())
            hash.Add(value);
        return hash.ToHashCode();
    }

    /// <summary>Structural equality operator.</summary>
    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        left is not null && right is not null && left.Equals(right);

    /// <summary>Structural inequality operator.</summary>
    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}
