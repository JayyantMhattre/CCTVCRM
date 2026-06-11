namespace Ashraak.SharedKernel.Interfaces;

/// <summary>
/// Abstraction over the system clock.
/// Injected wherever the current date/time is needed instead of calling
/// <c>DateTime.UtcNow</c> or <c>DateTimeOffset.UtcNow</c> directly.
/// </summary>
/// <remarks>
/// Replacing calls to static <c>DateTime.UtcNow</c> with this interface allows
/// unit tests to inject a deterministic <c>FakeDateTimeProvider</c> and assert on
/// time-sensitive logic (expiry checks, audit timestamps, token lifetimes) without
/// relying on wall-clock time.
/// </remarks>
public interface IDateTimeProvider
{
    /// <summary>Gets the current UTC date and time as a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/>.</summary>
    DateTime UtcNow { get; }

    /// <summary>Gets the current UTC date with no time component.</summary>
    DateOnly UtcToday { get; }

    /// <summary>Gets the current UTC date and time as a <see cref="DateTimeOffset"/> (offset always zero).</summary>
    DateTimeOffset UtcNowOffset { get; }
}
