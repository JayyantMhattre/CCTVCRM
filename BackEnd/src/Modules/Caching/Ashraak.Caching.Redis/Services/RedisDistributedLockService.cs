using Ashraak.Caching.Abstractions;
using StackExchange.Redis;

namespace Ashraak.Caching.Redis.Services;

/// <summary>
/// Redis-backed implementation of <see cref="IDistributedLock"/>.
/// Released atomically via a Lua script that checks the fencing token (<see cref="_value"/>)
/// before deleting the key, preventing another instance from releasing a lock it does not own.
/// </summary>
internal sealed class RedisDistributedLock : IDistributedLock
{
    private readonly IDatabase _redis;
    private readonly string _key;
    private readonly string _value;
    private bool _released;

    /// <summary>
    /// Creates a lock token. Called by <see cref="RedisDistributedLockService.AcquireAsync"/>.
    /// </summary>
    /// <param name="redis">The Redis database connection.</param>
    /// <param name="key">The Redis key holding the lock.</param>
    /// <param name="value">The unique fencing token value written when the lock was acquired.</param>
    /// <param name="isAcquired">Whether the lock was successfully acquired.</param>
    public RedisDistributedLock(IDatabase redis, string key, string value, bool isAcquired)
    {
        _redis = redis;
        _key = key;
        _value = value;
        IsAcquired = isAcquired;
        _released = !isAcquired;
    }

    /// <inheritdoc/>
    public bool IsAcquired { get; }

    /// <summary>
    /// Releases the distributed lock using a Lua CAS script.
    /// The script deletes the key only if its value matches the fencing token,
    /// guaranteeing that only the holder of the lock can release it.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (!_released && IsAcquired)
        {
            var script = LuaScript.Prepare(
                "if redis.call('get', @key) == @value then return redis.call('del', @key) else return 0 end");
            await _redis.ScriptEvaluateAsync(script, new { key = (RedisKey)_key, value = (RedisValue)_value });
            _released = true;
        }
    }
}

/// <summary>
/// Redis-backed implementation of <see cref="IDistributedLockService"/> using
/// the <c>SET NX EX</c> pattern (set-if-not-exists with TTL).
/// Suitable for Phase 1–2 concurrency control within a single Redis instance.
/// For multi-region scenarios (Phase 4), replace with Redlock algorithm.
/// </summary>
internal sealed class RedisDistributedLockService : IDistributedLockService
{
    private readonly IDatabase _redis;

    /// <summary>
    /// Initialises the service with the active Redis connection multiplexer.
    /// </summary>
    public RedisDistributedLockService(IConnectionMultiplexer connectionMultiplexer)
    {
        _redis = connectionMultiplexer.GetDatabase();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Retries on a <paramref name="retry"/> interval loop until <paramref name="wait"/> expires
    /// or the lock is acquired. Returns a non-acquired <see cref="IDistributedLock"/> rather than
    /// throwing, so callers can decide how to handle contention (abort, queue, return 409).
    /// </remarks>
    public async Task<IDistributedLock> AcquireAsync(
        string resource,
        TimeSpan expiry,
        TimeSpan? wait = null,
        TimeSpan? retry = null,
        CancellationToken cancellationToken = default)
    {
        var lockValue = Guid.NewGuid().ToString("N");
        var effectiveWait = wait ?? TimeSpan.FromSeconds(5);
        var effectiveRetry = retry ?? TimeSpan.FromMilliseconds(100);
        var deadline = DateTime.UtcNow.Add(effectiveWait);

        while (DateTime.UtcNow < deadline && !cancellationToken.IsCancellationRequested)
        {
            var acquired = await _redis.StringSetAsync(
                resource, lockValue, expiry, When.NotExists);

            if (acquired)
                return new RedisDistributedLock(_redis, resource, lockValue, isAcquired: true);

            await Task.Delay(effectiveRetry, cancellationToken);
        }

        return new RedisDistributedLock(_redis, resource, lockValue, isAcquired: false);
    }
}
