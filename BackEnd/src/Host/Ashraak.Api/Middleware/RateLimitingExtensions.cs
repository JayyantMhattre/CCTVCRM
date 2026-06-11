namespace Ashraak.Api.Middleware;

/// <summary>Registers Redis-backed rate limiting middleware.</summary>
public static class RateLimitingExtensions
{
    /// <summary>
    /// Applies config-driven rate limits for abuse-prone routes (auth token, register, etc.).
    /// Requires <see cref="Ashraak.Caching.Abstractions.ICacheService"/> (Caching module registered first).
    /// </summary>
    public static IApplicationBuilder UseRateLimitingMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<RateLimitingMiddleware>();
}
