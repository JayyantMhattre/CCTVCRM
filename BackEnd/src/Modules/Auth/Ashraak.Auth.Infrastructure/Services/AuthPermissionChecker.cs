using Ashraak.Auth.Infrastructure.Persistence.Authorization;
using Ashraak.Auth.Infrastructure.Persistence.Repositories;
using Ashraak.Caching.Abstractions;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Interfaces;

namespace Ashraak.Auth.Infrastructure.Services;

/// <summary>
/// Auth module implementation of <see cref="IAuthPermissionChecker"/>.
/// Resolves RBAC roles + ABAC permissions and caches the result using
/// the shared two-tier cache abstraction (memory + Redis).
/// </summary>
internal sealed class AuthPermissionChecker : IAuthPermissionChecker
{
    private static readonly TimeSpan PermissionCacheTtl = TimeSpan.FromMinutes(10);

    private readonly AuthAuthorizationRepository _authorizationRepository;
    private readonly ICacheService _cacheService;
    private readonly ITenantContext _tenantContext;

    public AuthPermissionChecker(
        AuthAuthorizationRepository authorizationRepository,
        ICacheService cacheService,
        ITenantContext tenantContext)
    {
        _authorizationRepository = authorizationRepository;
        _cacheService = cacheService;
        _tenantContext = tenantContext;
    }

    /// <inheritdoc />
    public async Task<bool> HasPermissionAsync(
        Guid userId,
        Guid tenantId,
        string permission,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await GetSnapshotAsync(userId, tenantId, cancellationToken);
        return snapshot.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public async Task<bool> IsInRoleAsync(
        Guid userId,
        Guid tenantId,
        string role,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await GetSnapshotAsync(userId, tenantId, cancellationToken);
        return snapshot.Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetPermissionsAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await GetSnapshotAsync(userId, tenantId, cancellationToken);
        return snapshot.Permissions;
    }

    /// <summary>
    /// Returns all roles for the user, sourced from the same cached authorisation snapshot.
    /// </summary>
    public async Task<IReadOnlyList<string>> GetRolesAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await GetSnapshotAsync(userId, tenantId, cancellationToken);
        return snapshot.Roles;
    }

    private async Task<AuthPermissionSnapshot> GetSnapshotAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeyBuilder.ForPermissions(tenantId, userId);

        return await _cacheService.GetOrSetAsync(
            cacheKey,
            async ct =>
            {
                await _authorizationRepository.EnsureUserBaselineAsync(userId, tenantId, ct);

                var roles = await _authorizationRepository.GetRolesAsync(userId, tenantId, ct);
                var grants = await _authorizationRepository.GetPermissionGrantsAsync(userId, tenantId, roles, ct);

                var resolvedPermissions = grants
                    .Where(IsAbacSatisfied)
                    .Select(x => x.Permission)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return new AuthPermissionSnapshot(roles, resolvedPermissions);
            },
            PermissionCacheTtl,
            cancellationToken);
    }

    /// <summary>
    /// Evaluates simple ABAC expressions in the form <c>key=value</c>.
    /// Supported keys:
    /// <list type="bullet">
    /// <item><description><c>plan</c> (matches <see cref="ITenantContext.Plan"/>).</description></item>
    /// <item><description><c>tenant</c> (matches current tenant identifier).</description></item>
    /// </list>
    /// </summary>
    private bool IsAbacSatisfied(AuthPermissionGrant grant)
    {
        if (string.IsNullOrWhiteSpace(grant.ConditionExpression))
            return true;

        var parts = grant.ConditionExpression.Split('=', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
            return false;

        var key = parts[0];
        var expected = parts[1];

        if (key.Equals("plan", StringComparison.OrdinalIgnoreCase))
            return string.Equals(_tenantContext.Plan, expected, StringComparison.OrdinalIgnoreCase);

        if (key.Equals("tenant", StringComparison.OrdinalIgnoreCase))
            return Guid.TryParse(expected, out var tenantId) && tenantId == _tenantContext.TenantId;

        return false;
    }
}
