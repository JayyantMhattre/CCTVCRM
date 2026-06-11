namespace Ashraak.Users.Domain.Aggregates.UserProfile;

/// <summary>
/// Strongly-typed identifier for the <see cref="UserProfile"/> aggregate root.
/// The underlying <see cref="Guid"/> value is identical to the corresponding
/// <c>AuthUser.Id.Value</c>, enabling cross-module correlation without a join table.
/// </summary>
/// <param name="Value">The underlying GUID value.</param>
public sealed record UserId(Guid Value)
{
    /// <summary>Creates a new <see cref="UserId"/> with a freshly generated GUID.</summary>
    public static UserId New() => new(Guid.NewGuid());

    /// <summary>Wraps an existing <see cref="Guid"/> as a <see cref="UserId"/>.</summary>
    /// <param name="value">The GUID to wrap.</param>
    public static UserId From(Guid value) => new(value);

    /// <inheritdoc/>
    public override string ToString() => Value.ToString();
}
