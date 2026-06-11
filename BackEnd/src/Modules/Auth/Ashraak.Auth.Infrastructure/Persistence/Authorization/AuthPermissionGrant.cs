namespace Ashraak.Auth.Infrastructure.Persistence.Authorization;

/// <summary>
/// Stores permission grants for either a role or a single user.
/// Optional <see cref="ConditionExpression"/> allows simple ABAC constraints
/// to be attached to a permission (for example: <c>plan=Enterprise</c>).
/// </summary>
internal sealed class AuthPermissionGrant
{
    /// <summary>Database identifier for this grant row.</summary>
    public Guid Id { get; set; }

    /// <summary>Tenant scope of this grant.</summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Role targeted by this grant; <see langword="null"/> when the grant is user-scoped.
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// User targeted by this grant; <see langword="null"/> when the grant is role-scoped.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>Permission identifier (for example: <c>tenant:read</c>).</summary>
    public string Permission { get; set; } = string.Empty;

    /// <summary>
    /// Optional ABAC condition expression in <c>key=value</c> format.
    /// Supported keys are handled in <c>AuthPermissionChecker</c>.
    /// </summary>
    public string? ConditionExpression { get; set; }

    /// <summary>UTC timestamp when the grant was created.</summary>
    public DateTime CreatedOnUtc { get; set; }
}
