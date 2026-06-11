namespace Ashraak.Tenant.Domain.Repositories;

/// <summary>
/// Repository port for the <see cref="Aggregates.Tenant.Tenant"/> aggregate root.
/// Implemented by <c>TenantRepository</c> in the Tenant Infrastructure layer.
/// </summary>
public interface ITenantRepository
{
    /// <summary>
    /// Returns the tenant with the given <paramref name="id"/>, or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="id">The strongly-typed tenant identifier.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<Aggregates.Tenant.Tenant?> GetByIdAsync(Aggregates.Tenant.TenantId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the tenant with the given URL <paramref name="slug"/>, or <see langword="null"/> if not found.
    /// Used to resolve the tenant from a subdomain or path prefix during request authentication.
    /// </summary>
    /// <param name="slug">The URL-safe slug (case-insensitive).</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<Aggregates.Tenant.Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns <see langword="true"/> when a tenant with the given <paramref name="slug"/> already exists.
    /// Used during provisioning to prevent duplicate slugs.
    /// </summary>
    /// <param name="slug">The slug to check.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>Tracks a new <see cref="Aggregates.Tenant.Tenant"/> for insertion.</summary>
    /// <param name="tenant">The new tenant to add.</param>
    void Add(Aggregates.Tenant.Tenant tenant);

    /// <summary>Marks an existing <see cref="Aggregates.Tenant.Tenant"/> as modified for update.</summary>
    /// <param name="tenant">The tenant entity to update.</param>
    void Update(Aggregates.Tenant.Tenant tenant);
}
