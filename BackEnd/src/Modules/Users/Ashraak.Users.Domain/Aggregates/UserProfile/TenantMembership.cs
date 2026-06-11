namespace Ashraak.Users.Domain.Aggregates.UserProfile;

/// <summary>
/// Owned entity within the <see cref="UserProfile"/> aggregate that records
/// a user's membership in a specific tenant and their assigned role.
/// </summary>
/// <remarks>
/// Modelled as a child entity (not a value object) because it has its own identity (<see cref="Id"/>)
/// and may have its role changed independently via <see cref="ChangeRole"/>.
/// Persisted in the <c>users.tenant_memberships</c> table, owned by <c>UserProfile</c>.
/// </remarks>
public sealed class TenantMembership
{
    private TenantMembership() { }

    /// <summary>Gets the surrogate primary key of this membership record.</summary>
    public Guid Id { get; private init; }

    /// <summary>Gets the identifier of the tenant this membership belongs to.</summary>
    public Guid TenantId { get; private init; }

    /// <summary>
    /// Gets the role assigned to the user in this tenant
    /// (e.g. <c>"Owner"</c>, <c>"Admin"</c>, <c>"Member"</c>).
    /// </summary>
    public string Role { get; private set; } = string.Empty;

    /// <summary>Gets the UTC timestamp of when the user joined this tenant.</summary>
    public DateTime JoinedOnUtc { get; private init; }

    /// <summary>
    /// Creates a new <see cref="TenantMembership"/> record.
    /// Called by <see cref="UserProfile.Create"/> and <see cref="UserProfile.AddMembership"/>.
    /// </summary>
    /// <param name="tenantId">The tenant being joined.</param>
    /// <param name="role">The initial role assigned to the user.</param>
    public static TenantMembership Create(Guid tenantId, string role) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        Role = role,
        JoinedOnUtc = DateTime.UtcNow
    };

    /// <summary>
    /// Updates the user's role in this tenant.
    /// A <c>RoleAssignedEvent</c> should be raised by the calling aggregate or application handler.
    /// </summary>
    /// <param name="newRole">The new role name.</param>
    public void ChangeRole(string newRole) => Role = newRole;
}
