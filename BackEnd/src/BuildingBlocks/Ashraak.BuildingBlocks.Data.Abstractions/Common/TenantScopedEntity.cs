namespace Ashraak.BuildingBlocks.Data.Abstractions.Common;

/// <summary>
/// Convenience base class that combines <see cref="BaseDataEntity"/> with
/// <see cref="ITenantScoped"/>.
///
/// <para>
/// Inherit from this class for any entity that must be isolated per tenant.
/// The <c>TenantId</c> will be automatically set during
/// <c>InsertAsync</c> / <c>InsertBulkAsync</c> from the active
/// <c>ITenantContext</c>, and automatically filtered on every read.
/// </para>
/// </summary>
public abstract class TenantScopedEntity : BaseDataEntity, ITenantScoped
{
    /// <inheritdoc />
    public Guid TenantId { get; set; }
}
