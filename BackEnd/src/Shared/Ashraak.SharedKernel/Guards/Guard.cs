using System.Runtime.CompilerServices;

namespace Ashraak.SharedKernel.Guards;

/// <summary>
/// Static helper class providing guard clauses for validating method arguments.
/// Guard clauses enforce preconditions at the boundary of a method and throw descriptive
/// exceptions rather than propagating invalid state deeper into the domain.
/// </summary>
/// <remarks>
/// The <see cref="CallerArgumentExpressionAttribute"/> automatically captures the
/// argument expression as the parameter name, so callers do not need to pass it manually:
/// <code>
/// Guard.AgainstNull(name);          // throws ArgumentNullException("name")
/// Guard.AgainstNullOrWhiteSpace(slug); // throws ArgumentException("slug cannot be ...")
/// </code>
/// </remarks>
public static class Guard
{
    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> when <paramref name="value"/> is <see langword="null"/>.
    /// Returns the non-null value for fluent chaining.
    /// </summary>
    /// <typeparam name="T">Reference type of the value being checked.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">Auto-captured expression name (do not pass explicitly).</param>
    /// <returns>The non-null value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static T AgainstNull<T>(
        T? value,
        [CallerArgumentExpression(nameof(value))] string parameterName = "") where T : class
    {
        if (value is null)
            throw new ArgumentNullException(parameterName);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/> is <see langword="null"/>,
    /// empty, or contains only whitespace. Returns the trimmed value on success.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="parameterName">Auto-captured expression name.</param>
    /// <returns>The original (non-trimmed) non-empty string.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null or whitespace.</exception>
    public static string AgainstNullOrWhiteSpace(
        string? value,
        [CallerArgumentExpression(nameof(value))] string parameterName = "")
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", parameterName);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/> is <see cref="Guid.Empty"/>.
    /// Returns the valid GUID on success.
    /// </summary>
    /// <param name="value">The GUID to check.</param>
    /// <param name="parameterName">Auto-captured expression name.</param>
    /// <returns>The non-empty GUID.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> equals <see cref="Guid.Empty"/>.</exception>
    public static Guid AgainstEmptyGuid(
        Guid value,
        [CallerArgumentExpression(nameof(value))] string parameterName = "")
    {
        if (value == Guid.Empty)
            throw new ArgumentException("GUID cannot be empty.", parameterName);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/> is zero or negative.
    /// Suitable for counts, prices, and other quantities that must be strictly positive.
    /// </summary>
    /// <typeparam name="T">A comparable numeric type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">Auto-captured expression name.</param>
    /// <returns>The positive value.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> ≤ 0.</exception>
    public static T AgainstNegativeOrZero<T>(
        T value,
        [CallerArgumentExpression(nameof(value))] string parameterName = "") where T : IComparable<T>
    {
        if (value.CompareTo(default) <= 0)
            throw new ArgumentException("Value must be greater than zero.", parameterName);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/> is negative.
    /// Suitable for values that can be zero but not negative (e.g. offset, version numbers).
    /// </summary>
    /// <typeparam name="T">A comparable numeric type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">Auto-captured expression name.</param>
    /// <returns>The non-negative value.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> &lt; 0.</exception>
    public static T AgainstNegative<T>(
        T value,
        [CallerArgumentExpression(nameof(value))] string parameterName = "") where T : IComparable<T>
    {
        if (value.CompareTo(default) < 0)
            throw new ArgumentException("Value cannot be negative.", parameterName);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/> exceeds <paramref name="maxLength"/> characters.
    /// Used in value object constructors to enforce database column length constraints early.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="maxLength">Maximum allowed length (inclusive).</param>
    /// <param name="parameterName">Auto-captured expression name.</param>
    /// <returns>The value if it does not exceed the max length.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is too long.</exception>
    public static string AgainstExceedingMaxLength(
        string value,
        int maxLength,
        [CallerArgumentExpression(nameof(value))] string parameterName = "")
    {
        if (value.Length > maxLength)
            throw new ArgumentException($"Value cannot exceed {maxLength} characters.", parameterName);
        return value;
    }
}
