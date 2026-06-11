namespace Ashraak.Api.Middleware;

/// <summary>Registers correlation ID middleware.</summary>
public static class CorrelationExtensions
{
    /// <summary>
    /// Reads or generates <c>X-Correlation-Id</c>, enriches Serilog and OpenTelemetry baggage,
    /// and echoes the header on the response.
    /// </summary>
    public static IApplicationBuilder UseCorrelationMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<CorrelationMiddleware>();
}
