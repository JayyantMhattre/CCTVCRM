using System.Security.Claims;
using Ashraak.ApiKeys.Application;
using Ashraak.SharedKernel.Contracts.ApiKeys.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Ashraak.ApiKeys.Infrastructure.Middleware;

/// <summary>
/// Authenticates machine requests via X-API-Key or Bearer ashk_* when JWT is not present.
/// </summary>
public sealed class ApiKeyAuthenticationMiddleware
{
    private static readonly PathString[] ManagementPaths =
    [
        new("/api/v1/api-keys"),
        new("/connect/token"),
        new("/health")
    ];

    private readonly RequestDelegate _next;
    private readonly ApiKeysOptions _options;

    public ApiKeyAuthenticationMiddleware(RequestDelegate next, IOptions<ApiKeysOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyValidator validator)
    {
        if (context.User.Identity?.IsAuthenticated == true || IsManagementPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var plaintextKey = ExtractApiKey(context);
        if (plaintextKey is null)
        {
            await _next(context);
            return;
        }

        var result = await validator.ValidateAsync(plaintextKey, context.RequestAborted);
        if (result is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Unauthorized",
                detail = "Invalid or expired API key.",
                status = StatusCodes.Status401Unauthorized
            }, context.RequestAborted);
            return;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.ApiKeyId.ToString()),
            new("sub", result.ApiKeyId.ToString()),
            new("tenant_id", result.TenantId.ToString()),
            new("tenantId", result.TenantId.ToString()),
            new("auth_type", "apikey"),
            new("api_key_id", result.ApiKeyId.ToString()),
            new("api_key_prefix", result.KeyPrefix)
        };

        foreach (var scope in result.Scopes)
            claims.Add(new Claim("permission", scope));

        var identity = new ClaimsIdentity(claims, "ApiKey");
        context.User = new ClaimsPrincipal(identity);
        context.Items["ApiKeyId"] = result.ApiKeyId;

        await _next(context);
    }

    private string? ExtractApiKey(HttpContext context)
    {
        if (_options.AllowHeaderAuthentication &&
            context.Request.Headers.TryGetValue("X-API-Key", out var header) &&
            !string.IsNullOrWhiteSpace(header))
        {
            return header.ToString();
        }

        if (_options.AllowBearerAuthentication)
        {
            var auth = context.Request.Headers.Authorization.ToString();
            if (auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = auth["Bearer ".Length..].Trim();
                if (token.StartsWith("ashk_", StringComparison.OrdinalIgnoreCase))
                    return token;
            }
        }

        return null;
    }

    private static bool IsManagementPath(PathString path) =>
        ManagementPaths.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));
}

public static class ApiKeyAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder app) =>
        app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
}
