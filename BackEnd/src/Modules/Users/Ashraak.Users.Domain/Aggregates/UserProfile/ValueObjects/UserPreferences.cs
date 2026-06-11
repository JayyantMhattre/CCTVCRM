using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Users.Domain.Aggregates.UserProfile.ValueObjects;

/// <summary>
/// Immutable value object containing user-level UI and notification preferences.
/// These are business/profile concerns and intentionally do not include any auth data.
/// </summary>
public sealed class UserPreferences : ValueObject
{
    private UserPreferences() { }

    /// <summary>Gets the preferred UI theme (<c>system</c>, <c>light</c>, or <c>dark</c>).</summary>
    public string Theme { get; private init; } = "system";

    /// <summary>Gets the preferred locale used by the UI (for example <c>en-US</c>).</summary>
    public string Locale { get; private init; } = "en-US";

    /// <summary>Gets the preferred timezone used in calendar/date displays.</summary>
    public string Timezone { get; private init; } = "UTC";

    /// <summary>Gets a value indicating whether product email notifications are enabled.</summary>
    public bool EmailNotificationsEnabled { get; private init; } = true;

    /// <summary>Gets the default preferences assigned to a newly created user profile.</summary>
    public static UserPreferences Default => new();

    /// <summary>
    /// Creates a new preferences value object with explicit values.
    /// </summary>
    public static UserPreferences Create(
        string theme,
        string locale,
        string timezone,
        bool emailNotificationsEnabled) => new()
        {
            Theme = theme,
            Locale = locale,
            Timezone = timezone,
            EmailNotificationsEnabled = emailNotificationsEnabled
        };

    /// <inheritdoc />
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Theme;
        yield return Locale;
        yield return Timezone;
        yield return EmailNotificationsEnabled;
    }
}
