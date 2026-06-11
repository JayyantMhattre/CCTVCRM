using Ashraak.SharedKernel.Interfaces;

namespace Ashraak.Api.Infrastructure;

/// <summary>
/// Production implementation of <see cref="IDateTimeProvider"/> that delegates
/// directly to the system clock.
/// </summary>
/// <remarks>
/// Registered as a singleton since there is no per-request state.
/// In tests, replace this with a <c>FakeDateTimeProvider</c> that returns a fixed
/// <see cref="DateTime"/> to make time-sensitive assertions deterministic.
/// </remarks>
internal sealed class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc/>
    public DateTime UtcNow => DateTime.UtcNow;

    /// <inheritdoc/>
    public DateOnly UtcToday => DateOnly.FromDateTime(DateTime.UtcNow);

    /// <inheritdoc/>
    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
}
