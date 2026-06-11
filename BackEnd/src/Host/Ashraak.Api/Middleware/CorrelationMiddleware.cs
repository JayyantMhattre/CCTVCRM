using System.Diagnostics;
using OpenTelemetry;
using Serilog.Context;

namespace Ashraak.Api.Middleware;

/// <summary>
/// Propagates <c>X-Correlation-Id</c> through Serilog and OpenTelemetry for end-to-end tracing.
/// </summary>
internal sealed class CorrelationMiddleware
{
    public const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(correlationId))
            correlationId = Guid.NewGuid().ToString("N");

        context.Items[HeaderName] = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            Activity.Current?.SetTag("correlation.id", correlationId);
            Baggage.SetBaggage("correlation.id", correlationId);

            await _next(context);
        }
    }
}
