namespace Ashraak.Tenant.Domain.Aggregates.Tenant;

/// <summary>
/// Strongly-typed identifier for the <see cref="Tenant"/> aggregate root.
/// Prevents accidental passing of a <c>Guid</c> intended for a different aggregate.
/// </summary>
/// <param name="Value">The underlying GUID value.</param>
public sealed record TenantId(Guid Value)
{
    /// <summary>Creates a new <see cref="TenantId"/> with a freshly generated GUID.</summary>
    public static TenantId New() => new(Guid.NewGuid());

    /// <summary>Wraps an existing <see cref="Guid"/> as a <see cref="TenantId"/>.</summary>
    /// <param name="value">The GUID to wrap.</param>
    public static TenantId From(Guid value) => new(value);

    /// <inheritdoc/>
    public override string ToString() => Value.ToString();
}
