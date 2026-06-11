using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Tenant.Domain.Aggregates.Tenant.ValueObjects;

/// <summary>
/// Value object holding all configurable security and localisation settings for a tenant.
/// Stored as a JSON column in the tenant table (or as flattened scalar columns via EF Core ownership).
/// </summary>
/// <remarks>
/// Immutable: create a new instance via <see cref="Create"/> or copy-modify via <c>with</c>
/// expressions on the private init properties when settings change.
/// </remarks>
public sealed class TenantSettings : ValueObject
{
    private TenantSettings() { }

    /// <summary>Gets the BCP 47 locale tag used for date/number formatting (e.g. <c>"en-US"</c>, <c>"de-DE"</c>).</summary>
    public string Locale { get; private init; } = "en-US";

    /// <summary>Gets the IANA time zone identifier for the tenant's primary timezone (e.g. <c>"UTC"</c>, <c>"America/New_York"</c>).</summary>
    public string Timezone { get; private init; } = "UTC";

    /// <summary>Gets the minimum password length enforced at registration and password-change time.</summary>
    public int PasswordMinLength { get; private init; } = 8;

    /// <summary>Gets a value indicating whether all users in this tenant must enable MFA to log in.</summary>
    public bool RequireMfa { get; private init; } = false;

    /// <summary>Gets the number of minutes before an idle session is invalidated.</summary>
    public int SessionTimeoutMinutes { get; private init; } = 480;

    /// <summary>
    /// Gets the default settings instance used when a tenant is first provisioned.
    /// </summary>
    public static TenantSettings Default => new();

    /// <summary>
    /// Creates a custom settings instance with the provided values.
    /// </summary>
    /// <param name="locale">BCP 47 locale tag.</param>
    /// <param name="timezone">IANA timezone identifier.</param>
    /// <param name="passwordMinLength">Minimum password length (default 8).</param>
    /// <param name="requireMfa">Whether tenant-wide MFA is required (default <see langword="false"/>).</param>
    /// <param name="sessionTimeoutMinutes">Idle session timeout in minutes (default 480 = 8 hours).</param>
    public static TenantSettings Create(
        string locale,
        string timezone,
        int passwordMinLength = 8,
        bool requireMfa = false,
        int sessionTimeoutMinutes = 480) => new()
        {
            Locale = locale,
            Timezone = timezone,
            PasswordMinLength = passwordMinLength,
            RequireMfa = requireMfa,
            SessionTimeoutMinutes = sessionTimeoutMinutes
        };

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Locale;
        yield return Timezone;
        yield return PasswordMinLength;
        yield return RequireMfa;
        yield return SessionTimeoutMinutes;
    }
}
