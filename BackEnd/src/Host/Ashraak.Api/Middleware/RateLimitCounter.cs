namespace Ashraak.Api.Middleware;

/// <summary>Redis-serialised counter state for a rate-limit window.</summary>
internal sealed class RateLimitCounter
{
    public int Count { get; set; }
    public DateTimeOffset WindowStart { get; set; }
}
