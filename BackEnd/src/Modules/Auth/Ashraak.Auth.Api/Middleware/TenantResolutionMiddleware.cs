using Ashraak.SharedKernel.Contracts.Tenant.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Ashraak.Auth.Api.Middleware;

/// <summary>
/// Resolves and validates tenant context for each request before module endpoints run.
/// Resolution sources: JWT claim (<c>tenant_id</c>/<c>tenantId</c>) and <c>X-Tenant-ID</c> header.
/// </summary>
/// <remarks>
/// The middleware enforces:
/// <list type="bullet">
/// <item><description>Authenticated requests must include a tenant identifier.</description></item>
/// <item><description>Header and token tenant must match when both are present.</description></item>
/// <item><description>The resolved tenant must be active according to <see cref="ITenantService"/>.</description></item>
/// </list>
/// </remarks>
public sealed class TenantResolutionMiddleware
{
    private static readonly PathString[] BypassPaths =
    {
        new("/health"),
        new("/connect"),
        new("/api/auth/register"),
        new("/api/v1/auth/register"),
        new("/api/auth/sso")
    };

    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Executes tenant resolution and request gating.
    /// </summary>
    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        if (ShouldBypass(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var tenantFromToken = context.User.FindFirst("tenant_id")?.Value
            ?? context.User.FindFirst("tenantId")?.Value;
        var tenantFromHeader = context.Request.Headers["X-Tenant-ID"].FirstOrDefault();

        var hasTokenTenant = Guid.TryParse(tenantFromToken, out var tokenTenantId);
        var hasHeaderTenant = Guid.TryParse(tenantFromHeader, out var headerTenantId);

        if (hasTokenTenant && hasHeaderTenant && tokenTenantId != headerTenantId)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "Tenant mismatch between token and header.");
            return;
        }

        var resolvedTenantId = hasTokenTenant ? tokenTenantId : (hasHeaderTenant ? headerTenantId : Guid.Empty);
        if (context.User.Identity?.IsAuthenticated == true && resolvedTenantId == Guid.Empty)
        {
            await WriteProblemAsync(context, StatusCodes.Status401Unauthorized, "Authenticated request is missing tenant context.");
            return;
        }

        if (resolvedTenantId != Guid.Empty && !await tenantService.IsActiveAsync(resolvedTenantId, context.RequestAborted))
        {
            await WriteProblemAsync(context, StatusCodes.Status403Forbidden, "The resolved tenant is not active.");
            return;
        }

        await _next(context);
    }

    private static bool ShouldBypass(PathString path) =>
        BypassPaths.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));

    private static async Task WriteProblemAsync(HttpContext context, int statusCode, string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            title = "Tenant resolution failed",
            detail,
            status = statusCode
        });
    }
}
