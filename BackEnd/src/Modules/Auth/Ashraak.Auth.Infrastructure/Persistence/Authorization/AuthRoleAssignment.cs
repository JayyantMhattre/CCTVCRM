namespace Ashraak.Auth.Infrastructure.Persistence.Authorization;

/// <summary>
/// Join entity that assigns a role name to a user within a specific tenant.
/// This is the RBAC anchor used by <c>AuthPermissionChecker</c> to resolve role membership.
/// </summary>
internal sealed class AuthRoleAssignment
{
    /// <summary>Database identifier for this assignment row.</summary>
    public Guid Id { get; set; }

    /// <summary>Tenant scope of the role assignment.</summary>
    public Guid TenantId { get; set; }

    /// <summary>User receiving the role.</summary>
    public Guid UserId { get; set; }

    /// <summary>Role name (for example: <c>Admin</c>, <c>Manager</c>, <c>Member</c>).</summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>UTC timestamp when the assignment was created.</summary>
    public DateTime CreatedOnUtc { get; set; }
}
