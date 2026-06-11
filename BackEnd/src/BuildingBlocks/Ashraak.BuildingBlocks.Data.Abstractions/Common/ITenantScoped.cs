namespace Ashraak.BuildingBlocks.Data.Abstractions.Common;

/// <summary>
/// Marker interface that signals a data entity belongs to a specific tenant.
///
/// <para>
/// When an entity implements this interface, <c>IDataRepository&lt;T&gt;</c>
/// automatically appends <c>WHERE TenantId = @currentTenantId</c> to every
/// query.  This ensures that tenants can never read or write each other's data
/// without any manual filtering in query/command handlers.
/// </para>
///
/// <para>
/// Prefer inheriting <see cref="TenantScopedEntity"/> (which already implements
/// this interface) rather than implementing <see cref="ITenantScoped"/> directly.
/// </para>
/// </summary>
public interface ITenantScoped
{
    /// <summary>The tenant that owns this record.</summary>
    Guid TenantId { get; }
}
