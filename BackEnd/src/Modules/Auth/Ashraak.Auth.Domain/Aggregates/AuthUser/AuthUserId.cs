namespace Ashraak.Auth.Domain.Aggregates.AuthUser;

/// <summary>
/// Strongly-typed identifier for the <see cref="AuthUser"/> aggregate root.
/// Using a dedicated record instead of a raw <see cref="Guid"/> prevents
/// accidental confusion between <c>AuthUserId</c>, <c>UserId</c>, and <c>TenantId</c>
/// at compile time.
/// </summary>
/// <param name="Value">The underlying GUID value.</param>
public sealed record AuthUserId(Guid Value)
{
    /// <summary>Creates a new <see cref="AuthUserId"/> with a freshly generated GUID.</summary>
    public static AuthUserId New() => new(Guid.NewGuid());

    /// <summary>Wraps an existing <see cref="Guid"/> as an <see cref="AuthUserId"/>.</summary>
    /// <param name="value">The GUID to wrap.</param>
    public static AuthUserId From(Guid value) => new(value);

    /// <inheritdoc/>
    public override string ToString() => Value.ToString();
}
