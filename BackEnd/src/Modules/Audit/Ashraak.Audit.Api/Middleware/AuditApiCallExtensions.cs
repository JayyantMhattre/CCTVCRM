using Microsoft.AspNetCore.Builder;

namespace Ashraak.Audit.Api.Middleware;

/// <summary>
/// Pipeline extension for HTTP API call audit capture.
/// </summary>
public static class AuditApiCallExtensions
{
    /// <summary>
    /// Adds audit API call logging middleware to the request pipeline.
    /// </summary>
    public static IApplicationBuilder UseAuditApiCallLogging(this IApplicationBuilder app) =>
        app.UseMiddleware<AuditApiCallMiddleware>();
}
