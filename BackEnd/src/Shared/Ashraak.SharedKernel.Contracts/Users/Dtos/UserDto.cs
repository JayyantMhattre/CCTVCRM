namespace Ashraak.SharedKernel.Contracts.Users.Dtos;

/// <summary>
/// Lifecycle status of a user profile within a tenant.
/// </summary>
public enum UserStatus
{
    /// <summary>The user can log in and perform actions.</summary>
    Active,

    /// <summary>The user account exists but is not permitted to log in (e.g. self-deactivated).</summary>
    Inactive,

    /// <summary>The user account has been suspended by an administrator.</summary>
    Suspended
}

/// <summary>
/// Read-only projection of a <c>UserProfile</c> aggregate exposed across module boundaries.
/// Returned by <c>IUserService</c> and used by other modules that need user identity data.
/// </summary>
/// <param name="UserId">Unique identifier of the user (matches the <c>AuthUser</c> identifier).</param>
/// <param name="TenantId">The tenant this user belongs to.</param>
/// <param name="Email">The user's primary email address.</param>
/// <param name="DisplayName">The name shown in the UI (e.g. <c>"Jane Smith"</c>).</param>
/// <param name="AvatarUrl">Optional URL to the user's profile avatar image.</param>
/// <param name="Preferences">User-specific product preferences (theme/locale/timezone/notifications).</param>
/// <param name="Status">Current lifecycle status of the user.</param>
/// <param name="CreatedOnUtc">UTC timestamp of when the user profile was created.</param>
public sealed record UserDto(
    Guid UserId,
    Guid TenantId,
    string Email,
    string DisplayName,
    string? AvatarUrl,
    UserPreferencesDto Preferences,
    UserStatus Status,
    DateTime CreatedOnUtc);

/// <summary>
/// Cross-module representation of user profile preferences.
/// This remains part of profile/business data and excludes authentication configuration.
/// </summary>
/// <param name="Theme">Preferred theme.</param>
/// <param name="Locale">Preferred locale.</param>
/// <param name="Timezone">Preferred timezone.</param>
/// <param name="EmailNotificationsEnabled">Whether email notifications are enabled.</param>
public sealed record UserPreferencesDto(
    string Theme,
    string Locale,
    string Timezone,
    bool EmailNotificationsEnabled);
