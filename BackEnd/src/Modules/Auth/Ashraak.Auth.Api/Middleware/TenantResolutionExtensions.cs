using Microsoft.AspNetCore.Builder;

namespace Ashraak.Auth.Api.Middleware;

/// <summary>
/// Pipeline extension for tenant resolution middleware.
/// </summary>
public static class TenantResolutionExtensions
{
    /// <summary>
    /// Adds tenant resolution/validation to the HTTP pipeline.
    /// </summary>
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app) =>
        app.UseMiddleware<TenantResolutionMiddleware>();
}
