using System.Text.RegularExpressions;

namespace Ashraak.SharedKernel.Extensions;

/// <summary>
/// Extension methods for <see cref="string"/> that are used across domain and application layers.
/// All methods are pure functions with no side effects.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// Converts a string to a URL-safe slug by lowercasing, trimming, replacing spaces with hyphens,
    /// and stripping all characters that are not lowercase letters, digits, or hyphens.
    /// </summary>
    /// <param name="value">The input string (e.g. a tenant name <c>"Acme Corp!"</c>).</param>
    /// <returns>A slug suitable for use in URLs (e.g. <c>"acme-corp"</c>).</returns>
    public static string ToSlug(this string value) =>
        SlugifyRegex().Replace(value.ToLowerInvariant().Trim().Replace(' ', '-'), string.Empty);

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="value"/> is a syntactically valid email address.
    /// Uses a lightweight regex; does not verify deliverability.
    /// </summary>
    /// <param name="value">The string to test.</param>
    public static bool IsValidEmail(this string value) =>
        !string.IsNullOrWhiteSpace(value) && EmailRegex().IsMatch(value);

    /// <summary>
    /// Truncates <paramref name="value"/> to at most <paramref name="maxLength"/> characters,
    /// appending <paramref name="suffix"/> when truncation occurs.
    /// </summary>
    /// <param name="value">The string to truncate.</param>
    /// <param name="maxLength">Maximum length of the resulting string including the suffix.</param>
    /// <param name="suffix">Ellipsis or other indicator appended on truncation. Defaults to <c>"..."</c>.</param>
    /// <returns>The original string if it fits, otherwise a truncated string with the suffix.</returns>
    public static string Truncate(this string value, int maxLength, string suffix = "...") =>
        value.Length <= maxLength ? value : string.Concat(value.AsSpan(0, maxLength - suffix.Length), suffix);

    /// <summary>
    /// Returns <see langword="null"/> when <paramref name="value"/> is <see langword="null"/>,
    /// empty, or whitespace-only; otherwise returns the original value.
    /// Useful for normalising optional string inputs before persistence.
    /// </summary>
    /// <param name="value">The string to test.</param>
    public static string? NullIfEmpty(this string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;

    [GeneratedRegex(@"[^a-z0-9\-]")]
    private static partial Regex SlugifyRegex();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
