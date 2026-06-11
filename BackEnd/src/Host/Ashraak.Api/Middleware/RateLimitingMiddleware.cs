using Ashraak.Api.Options;
using Ashraak.Caching.Abstractions;
using Microsoft.Extensions.Options;

namespace Ashraak.Api.Middleware;

/// <summary>
/// Lightweight Redis-backed rate limiting by tenant, route group, and client IP.
/// </summary>
internal sealed class RateLimitingMiddleware
{
    private static readonly PathString[] BypassPrefixes =
    [
        new("/health"),
        new("/scalar"),
        new("/openapi")
    ];

    private readonly RequestDelegate _next;
    private readonly ICacheService _cache;
    private readonly RateLimitingOptions _options;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    public RateLimitingMiddleware(
        RequestDelegate next,
        ICacheService cache,
        IOptions<RateLimitingOptions> options,
        ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_options.Enabled || ShouldBypass(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var route = FindMatchingRoute(context);
        if (route is null)
        {
            await _next(context);
            return;
        }

        var (routeKey, policy) = route.Value;
        var tenantId = ResolveTenantId(context);
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var apiKeyId = context.User.FindFirst("api_key_id")?.Value ?? string.Empty;
        var rateSubject = string.IsNullOrEmpty(apiKeyId) ? clientIp : $"apikey:{apiKeyId}";
        var cacheKey = CacheKeyBuilder.ForRateLimit(tenantId, routeKey, rateSubject);
        var window = TimeSpan.FromSeconds(Math.Max(1, policy.WindowSeconds));

        var counter = await _cache.GetAsync<RateLimitCounter>(cacheKey, context.RequestAborted)
                      ?? new RateLimitCounter { Count = 0, WindowStart = DateTimeOffset.UtcNow };

        var now = DateTimeOffset.UtcNow;
        if (now - counter.WindowStart >= window)
        {
            counter = new RateLimitCounter { Count = 1, WindowStart = now };
            await _cache.SetAsync(cacheKey, counter, window, context.RequestAborted);
            await _next(context);
            return;
        }

        counter.Count++;
        var remaining = window - (now - counter.WindowStart);
        await _cache.SetAsync(cacheKey, counter, remaining > TimeSpan.Zero ? remaining : window, context.RequestAborted);

        if (counter.Count > policy.Limit)
        {
            var retryAfterSeconds = (int)Math.Ceiling(remaining.TotalSeconds);
            if (retryAfterSeconds < 1)
                retryAfterSeconds = policy.WindowSeconds;

            _logger.LogWarning(
                "Rate limit exceeded for {RouteKey} tenant {TenantId} ip {ClientIp} ({Count}/{Limit})",
                routeKey, tenantId, clientIp, counter.Count, policy.Limit);

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers.RetryAfter = retryAfterSeconds.ToString();
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Too many requests",
                detail = $"Rate limit exceeded for route group '{routeKey}'.",
                status = StatusCodes.Status429TooManyRequests,
                retryAfterSeconds
            }, context.RequestAborted);
            return;
        }

        await _next(context);
    }

    private (string RouteKey, RateLimitRouteOptions Policy)? FindMatchingRoute(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        foreach (var (routeKey, policy) in _options.Routes)
        {
            if (string.IsNullOrWhiteSpace(policy.Path))
                continue;

            if (!path.StartsWith(policy.Path, StringComparison.OrdinalIgnoreCase))
                continue;

            if (policy.Methods.Length > 0 &&
                !policy.Methods.Contains(context.Request.Method, StringComparer.OrdinalIgnoreCase))
                continue;

            return (routeKey, policy);
        }

        return null;
    }

    private static Guid ResolveTenantId(HttpContext context)
    {
        if (Guid.TryParse(
                context.User.FindFirst("tenant_id")?.Value
                ?? context.User.FindFirst("tenantId")?.Value,
                out var tokenTenant))
            return tokenTenant;

        if (Guid.TryParse(context.Request.Headers["X-Tenant-ID"].FirstOrDefault(), out var headerTenant))
            return headerTenant;

        return Guid.Empty;
    }

    private static bool ShouldBypass(PathString path) =>
        BypassPrefixes.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));
}
