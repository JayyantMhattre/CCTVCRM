using Ashraak.Caching.Abstractions;
using Ashraak.SharedKernel.Contracts.Tenant.Dtos;
using Ashraak.SharedKernel.Contracts.Tenant.Interfaces;
using Ashraak.Tenant.Application.Mapping;
using Ashraak.Tenant.Domain.Repositories;

using ContractsPlan = Ashraak.SharedKernel.Contracts.Tenant.Dtos.TenantPlan;
using ContractsStatus = Ashraak.SharedKernel.Contracts.Tenant.Dtos.TenantStatus;

namespace Ashraak.Tenant.Infrastructure.Services;

/// <summary>
/// Implementation of <see cref="ITenantService"/> — the cross-module read facade for the Tenant module.
/// Uses a 5-minute Redis cache (via <see cref="ICacheService.GetOrSetAsync"/>) to avoid repeated
/// database round trips from the Users, Auth, and Audit modules.
/// </summary>
/// <remarks>
/// Cache invalidation: when a tenant's plan or status changes, the corresponding domain event handlers
/// must call <c>ICacheService.RemoveAsync</c> with the tenant's cache key to keep the cross-module
/// view consistent.
/// </remarks>
internal sealed class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ICacheService _cacheService;

    /// <summary>Initialises the service with its repository and cache dependencies.</summary>
    public TenantService(ITenantRepository tenantRepository, ICacheService cacheService)
    {
        _tenantRepository = tenantRepository;
        _cacheService = cacheService;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Cache key: <c>CacheKeyBuilder.ForEntity(tenantId, "tenant", "config", tenantId)</c>.
    /// TTL: 5 minutes.
    /// </remarks>
    public async Task<TenantDto?> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeyBuilder.ForEntity(tenantId, "tenant", "config", tenantId);

        return await _cacheService.GetOrSetAsync(
            cacheKey,
            async ct =>
            {
                var tenant = await _tenantRepository.GetByIdAsync(
                    new Domain.Aggregates.Tenant.TenantId(tenantId), ct);

                if (tenant is null)
                    return null!;

                return new TenantDto(
                    tenant.Id.Value,
                    tenant.Name,
                    tenant.Slug,
                    (ContractsPlan)(int)tenant.Plan,
                    (ContractsStatus)(int)tenant.Status,
                    tenant.CustomDomain,
                    tenant.CreatedOnUtc);
            },
            TimeSpan.FromMinutes(5),
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> IsActiveAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await GetTenantAsync(tenantId, cancellationToken);
        return tenant?.Status == TenantStatus.Active;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Feature flags are stored separately in the Redis cache as a <c>Dictionary&lt;string, bool&gt;</c>.
    /// They are written by the feature-flag management endpoint (future) and read here.
    /// Returns <see langword="false"/> when no flag dictionary exists for the tenant.
    /// </remarks>
    public async Task<bool> GetFeatureFlagAsync(Guid tenantId, string flagName, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeyBuilder.ForFeatureFlags(tenantId);
        var flags = await _cacheService.GetAsync<Dictionary<string, bool>>(cacheKey, cancellationToken);
        return flags?.TryGetValue(flagName, out var value) == true && value;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Goes directly to the database to always return the authoritative seat limit,
    /// bypassing the DTO cache which does not include the <c>Subscription</c> value object.
    /// </remarks>
    public async Task<int> GetSeatLimitAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(
            new Domain.Aggregates.Tenant.TenantId(tenantId), cancellationToken);
        return tenant?.Subscription.SeatLimit ?? 0;
    }

    /// <inheritdoc/>
    public async Task<TenantSettingsDto?> GetSettingsAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(
            new Domain.Aggregates.Tenant.TenantId(tenantId), cancellationToken);

        return tenant is null
            ? null
            : TenantSettingsMapper.ToDto(tenant.Settings);
    }
}
