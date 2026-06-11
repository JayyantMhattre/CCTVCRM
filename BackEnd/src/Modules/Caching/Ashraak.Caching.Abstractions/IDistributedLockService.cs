namespace Ashraak.Caching.Abstractions;

/// <summary>
/// Represents a distributed lock token. Disposing this releases the lock in Redis.
/// </summary>
/// <remarks>
/// Always dispose within a <c>using</c> declaration or <c>await using</c> block.
/// Check <see cref="IsAcquired"/> before performing the guarded operation.
/// </remarks>
public interface IDistributedLock : IAsyncDisposable
{
    /// <summary>
    /// Gets a value indicating whether this instance holds the distributed lock.
    /// If <see langword="false"/>, the lock was not acquired within the wait window;
    /// the caller should abort or return a conflict response.
    /// </summary>
    bool IsAcquired { get; }
}

/// <summary>
/// Provides distributed mutual exclusion using Redis <c>SET NX EX</c>.
/// Used to prevent race conditions in critical sections that span multiple
/// application instances (e.g. tenant provisioning, seat-limit enforcement).
/// </summary>
/// <remarks>
/// The implementation uses a Lua script for atomic lock release to avoid accidental
/// deletion of a lock acquired by another instance (fencing token pattern).
/// </remarks>
public interface IDistributedLockService
{
    /// <summary>
    /// Attempts to acquire a distributed lock on <paramref name="resource"/>.
    /// Retries with <paramref name="retry"/> intervals until <paramref name="wait"/> elapses
    /// or the lock is obtained.
    /// </summary>
    /// <param name="resource">
    /// The unique lock key in Redis (use <see cref="CacheKeyBuilder.ForDistributedLock"/>).
    /// </param>
    /// <param name="expiry">
    /// How long the lock is held before Redis auto-releases it.
    /// Should exceed the expected duration of the guarded operation to avoid premature release.
    /// </param>
    /// <param name="wait">Maximum time to wait for the lock. Defaults to 5 seconds.</param>
    /// <param name="retry">
    /// Interval between acquisition retries. Defaults to 100 milliseconds.
    /// </param>
    /// <param name="cancellationToken">Propagates notification that the wait should be abandoned.</param>
    /// <returns>
    /// An <see cref="IDistributedLock"/> whose <see cref="IDistributedLock.IsAcquired"/> indicates
    /// whether the lock was obtained. Always dispose the returned instance.
    /// </returns>
    Task<IDistributedLock> AcquireAsync(
        string resource,
        TimeSpan expiry,
        TimeSpan? wait = null,
        TimeSpan? retry = null,
        CancellationToken cancellationToken = default);
}
