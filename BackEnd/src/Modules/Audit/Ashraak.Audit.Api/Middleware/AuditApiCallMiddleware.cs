using System.Text.Json;
using Ashraak.SharedKernel.Contracts.Audit.Dtos;
using Ashraak.SharedKernel.Contracts.Audit.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Ashraak.Audit.Api.Middleware;

/// <summary>
/// Captures HTTP API call metadata and emits audit entries asynchronously.
/// Designed to be lightweight and non-blocking by delegating persistence to the audit queue.
/// </summary>
public sealed class AuditApiCallMiddleware
{
    private static readonly PathString[] ExcludedPaths =
    {
        new("/health"),
        new("/connect"),
        new("/api/audit-logs")
    };

    private readonly RequestDelegate _next;

    public AuditApiCallMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Executes the middleware and logs API call data after request completion.
    /// </summary>
    public async Task InvokeAsync(
        HttpContext context,
        IAuditService auditService,
        ITenantContext tenantContext,
        ICurrentUser currentUser)
    {
        if (ShouldSkip(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var startedAt = DateTime.UtcNow;
        Exception? exception = null;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            var tenantId = tenantContext.TenantId;
            if (tenantId != Guid.Empty)
            {
                var outcome = new
                {
                    Method = context.Request.Method,
                    Path = context.Request.Path.Value,
                    QueryString = context.Request.QueryString.Value,
                    StatusCode = context.Response.StatusCode,
                    StartedAtUtc = startedAt,
                    FinishedAtUtc = DateTime.UtcNow,
                    DurationMs = (DateTime.UtcNow - startedAt).TotalMilliseconds,
                    Exception = exception?.GetType().Name,
                    ExceptionMessage = exception?.Message
                };

                var audit = new AuditEntryDto(
                    TenantId: tenantId,
                    UserId: currentUser.IsAuthenticated ? currentUser.UserId : null,
                    Module: "API",
                    Action: "ApiCall",
                    EntityType: "HttpRequest",
                    EntityId: $"{context.Request.Method}:{context.Request.Path}",
                    OldValues: null,
                    NewValues: JsonSerializer.Serialize(outcome),
                    IpAddress: context.Connection.RemoteIpAddress?.ToString(),
                    UserAgent: context.Request.Headers.UserAgent.FirstOrDefault(),
                    OccurredOnUtc: startedAt);

                await auditService.LogAsync(audit, context.RequestAborted);
            }
        }
    }

    private static bool ShouldSkip(PathString path) =>
        ExcludedPaths.Any(x => path.StartsWithSegments(x, StringComparison.OrdinalIgnoreCase));
}
