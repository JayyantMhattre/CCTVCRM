using Ashraak.Auth.Infrastructure.Persistence.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Auth.Infrastructure.Persistence.Repositories;

/// <summary>
/// Persistence helper for RBAC/ABAC data used by the Auth module.
/// Reads role assignments and permission grants from the Auth schema.
/// </summary>
internal sealed class AuthAuthorizationRepository
{
    private readonly AuthDbContext _context;

    public AuthAuthorizationRepository(AuthDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns all role names assigned to the user in the tenant.
    /// </summary>
    public async Task<IReadOnlyList<string>> GetRolesAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RoleAssignments
            .Where(x => x.UserId == userId && x.TenantId == tenantId)
            .OrderBy(x => x.RoleName)
            .Select(x => x.RoleName)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Returns all permission grants that apply to the user, both direct and role-based.
    /// </summary>
    public async Task<IReadOnlyList<AuthPermissionGrant>> GetPermissionGrantsAsync(
        Guid userId,
        Guid tenantId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default)
    {
        var roleList = roles.Count == 0 ? Array.Empty<string>() : roles.ToArray();

        return await _context.PermissionGrants
            .Where(x => x.TenantId == tenantId
                && ((x.UserId.HasValue && x.UserId == userId)
                    || (!string.IsNullOrWhiteSpace(x.RoleName) && roleList.Contains(x.RoleName!))))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Seeds a default <c>Member</c> role assignment and baseline permissions
    /// for a user if they currently have no roles.
    /// </summary>
    public async Task EnsureUserBaselineAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var hasRole = await _context.RoleAssignments
            .AnyAsync(x => x.UserId == userId && x.TenantId == tenantId, cancellationToken);

        if (!hasRole)
        {
            _context.RoleAssignments.Add(new AuthRoleAssignment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TenantId = tenantId,
                RoleName = "Member",
                CreatedOnUtc = DateTime.UtcNow
            });
        }

        var baselinePermissions = new[] { "tenant:read", "user:read" };
        foreach (var permission in baselinePermissions)
        {
            var exists = await _context.PermissionGrants.AnyAsync(
                x => x.TenantId == tenantId && x.RoleName == "Member" && x.Permission == permission,
                cancellationToken);

            if (!exists)
            {
                _context.PermissionGrants.Add(new AuthPermissionGrant
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    RoleName = "Member",
                    UserId = null,
                    Permission = permission,
                    ConditionExpression = null,
                    CreatedOnUtc = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Assigns a role to a user within a tenant if not already assigned.</summary>
    public async Task AssignRoleAsync(
        Guid userId,
        Guid tenantId,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        var exists = await _context.RoleAssignments.AnyAsync(
            x => x.UserId == userId && x.TenantId == tenantId && x.RoleName == roleName,
            cancellationToken);

        if (exists)
            return;

        _context.RoleAssignments.Add(new AuthRoleAssignment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TenantId = tenantId,
            RoleName = roleName,
            CreatedOnUtc = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);
    }
}
