namespace Ashraak.Users.Domain.Enums;

/// <summary>
/// Lifecycle status of a <c>UserProfile</c> within the Users domain.
/// Integer values match <c>SharedKernel.Contracts.Users.Dtos.UserStatus</c> to allow
/// safe explicit casting between the domain and contract enums.
/// </summary>
public enum UserStatus
{
    /// <summary>The user profile is active and the user can log in.</summary>
    Active,

    /// <summary>The user profile has been deactivated; login is prevented.</summary>
    Inactive,

    /// <summary>The user has been administratively suspended within the tenant.</summary>
    Suspended
}
