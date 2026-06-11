namespace Ashraak.Api.Options;

/// <summary>
/// Configuration for Redis-backed request rate limiting.
/// </summary>
public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    /// <summary>When false, rate limiting middleware is a no-op.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Route groups keyed by configuration name (e.g. <c>auth/token</c>).</summary>
    public Dictionary<string, RateLimitRouteOptions> Routes { get; set; } = new();
}

/// <summary>Per-route rate limit policy.</summary>
public sealed class RateLimitRouteOptions
{
    /// <summary>Request path prefix to match (e.g. <c>/connect/token</c>).</summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>HTTP methods to limit. Empty = all methods.</summary>
    public string[] Methods { get; set; } = [];

    /// <summary>Maximum requests allowed per window.</summary>
    public int Limit { get; set; } = 10;

    /// <summary>Sliding/fixed window length in seconds.</summary>
    public int WindowSeconds { get; set; } = 60;
}
