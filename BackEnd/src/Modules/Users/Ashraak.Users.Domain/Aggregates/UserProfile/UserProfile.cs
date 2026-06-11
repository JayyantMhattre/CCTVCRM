using Ashraak.SharedKernel.Domain.Primitives;
using Ashraak.Users.Domain.Aggregates.UserProfile.Events;
using Ashraak.Users.Domain.Aggregates.UserProfile.ValueObjects;
using Ashraak.Users.Domain.Enums;

namespace Ashraak.Users.Domain.Aggregates.UserProfile;

/// <summary>
/// Aggregate root for the Users module representing a user's public profile and
/// their membership in one or more tenants.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="UserProfile"/> shares its <c>Guid</c> identifier with <c>AuthUser</c> in the Auth module.
/// This identity correlation is intentional — the same GUID is written to both records during registration,
/// enabling cross-module lookups by Id without a shared foreign key table.
/// </para>
/// <para>
/// A user may be a member of multiple tenants via the <see cref="Memberships"/> collection.
/// The primary <see cref="TenantId"/> records the tenant that provisioned the account.
/// </para>
/// </remarks>
public sealed class UserProfile : AggregateRoot<UserId>
{
    private readonly List<TenantMembership> _memberships = [];

    private UserProfile(UserId id) : base(id) { }

    /// <summary>Gets the primary tenant this user was registered under.</summary>
    public Guid TenantId { get; private set; }

    /// <summary>Gets the user's email address (as registered in the Auth module).</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Gets the user's display name shown in the UI.</summary>
    public string DisplayName { get; private set; } = string.Empty;

    /// <summary>Gets the optional URL to the user's avatar image.</summary>
    public string? AvatarUrl { get; private set; }

    /// <summary>
    /// Gets the user's personal, non-authentication preferences (theme, locale, timezone, notifications).
    /// </summary>
    public UserPreferences Preferences { get; private set; } = UserPreferences.Default;

    /// <summary>Gets the user's current lifecycle status.</summary>
    public UserStatus Status { get; private set; }

    /// <summary>Gets the list of tenant memberships for this user.</summary>
    public IReadOnlyList<TenantMembership> Memberships => _memberships.AsReadOnly();

    /// <summary>Gets the UTC timestamp when this profile was created.</summary>
    public DateTime CreatedOnUtc { get; private set; }

    /// <summary>Gets the UTC timestamp of the most recent update.</summary>
    public DateTime UpdatedOnUtc { get; private set; }

    /// <summary>
    /// Factory method that creates a new active user profile, adds the initial tenant membership
    /// with the <c>"Member"</c> role, and raises <see cref="UserProfileCreatedDomainEvent"/>.
    /// </summary>
    /// <param name="userId">
    /// The shared identifier assigned during Auth module registration.
    /// Must match the <c>AuthUser.Id</c> of the same user.
    /// </param>
    /// <param name="tenantId">The tenant this user belongs to.</param>
    /// <param name="email">The user's email address (copied from Auth for read convenience).</param>
    /// <param name="displayName">The display name provided during registration.</param>
    public static UserProfile Create(Guid userId, Guid tenantId, string email, string displayName)
    {
        var profile = new UserProfile(UserId.From(userId))
        {
            TenantId = tenantId,
            Email = email,
            DisplayName = displayName,
            Preferences = UserPreferences.Default,
            Status = UserStatus.Active,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };

        profile._memberships.Add(TenantMembership.Create(tenantId, "Member"));

        profile.RaiseDomainEvent(new UserProfileCreatedDomainEvent(
            userId, tenantId, email, displayName));

        return profile;
    }

    /// <summary>
    /// Updates the user's mutable profile fields.
    /// Does not affect authentication credentials managed by the Auth module.
    /// </summary>
    /// <param name="displayName">The new display name (max 100 characters).</param>
    /// <param name="avatarUrl">The new avatar URL, or <see langword="null"/> to clear it.</param>
    public void UpdateProfile(string displayName, string? avatarUrl)
    {
        DisplayName = displayName;
        AvatarUrl = avatarUrl;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Replaces the user's preference set with a new immutable value object.
    /// </summary>
    /// <param name="preferences">The new preference values to persist.</param>
    public void UpdatePreferences(UserPreferences preferences)
    {
        Preferences = preferences;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the user profile and raises <see cref="UserDeactivatedDomainEvent"/>.
    /// Handled by the Auth module to revoke active tokens.
    /// Idempotent if already <see cref="UserStatus.Inactive"/>.
    /// </summary>
    public void Deactivate()
    {
        if (Status == UserStatus.Inactive)
            return;

        Status = UserStatus.Inactive;
        UpdatedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new UserDeactivatedDomainEvent(Id.Value, TenantId));
    }

    /// <summary>
    /// Adds a new <see cref="TenantMembership"/> for this user.
    /// Idempotent: if the user is already a member of <paramref name="tenantId"/>, the call is ignored.
    /// </summary>
    /// <param name="tenantId">The tenant to add membership for.</param>
    /// <param name="role">The role assigned to the user in the new tenant.</param>
    public void AddMembership(Guid tenantId, string role)
    {
        if (_memberships.Any(m => m.TenantId == tenantId))
            return;

        _memberships.Add(TenantMembership.Create(tenantId, role));
        UpdatedOnUtc = DateTime.UtcNow;
    }
}
