namespace Ashraak.SharedKernel.Extensions;

/// <summary>
/// Extension methods for <see cref="DateTime"/> and <see cref="DateTimeOffset"/>
/// used across domain and application layers for time-based logic.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="dateTime"/> is in the past relative to UTC now.
    /// Used for checking token and subscription expiry.
    /// </summary>
    /// <param name="dateTime">A UTC <see cref="DateTime"/> to evaluate.</param>
    public static bool IsExpired(this DateTime dateTime) => dateTime < DateTime.UtcNow;

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="dateTimeOffset"/> is in the past relative to UTC now.
    /// </summary>
    /// <param name="dateTimeOffset">A <see cref="DateTimeOffset"/> (offset-independent comparison).</param>
    public static bool IsExpired(this DateTimeOffset dateTimeOffset) => dateTimeOffset < DateTimeOffset.UtcNow;

    /// <summary>
    /// Returns a new <see cref="DateTime"/> set to midnight (00:00:00.000) on the same day,
    /// preserving <see cref="DateTimeKind.Utc"/>.
    /// </summary>
    /// <param name="dateTime">The source date.</param>
    public static DateTime StartOfDay(this DateTime dateTime) =>
        new(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Returns a new <see cref="DateTime"/> set to the last millisecond of the day (23:59:59.999),
    /// preserving <see cref="DateTimeKind.Utc"/>.
    /// Useful for building inclusive date-range filters.
    /// </summary>
    /// <param name="dateTime">The source date.</param>
    public static DateTime EndOfDay(this DateTime dateTime) =>
        new(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999, DateTimeKind.Utc);
}
