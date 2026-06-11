using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ashraak.Api.Middleware;

/// <summary>
/// ASP.NET Core exception handler that catches any unhandled exception propagating
/// through the middleware pipeline and converts it to a RFC 7807 <see cref="ProblemDetails"/>
/// response with HTTP 500.
/// </summary>
/// <remarks>
/// <para>
/// Registered via <c>builder.Services.AddExceptionHandler&lt;GlobalExceptionHandler&gt;()</c>
/// and activated with <c>app.UseExceptionHandler()</c> in <c>Program.cs</c>.
/// </para>
/// <para>
/// The response body never exposes internal exception details to the client in production.
/// Only the <c>traceId</c> extension is included so that support teams can correlate
/// the error to a Serilog/OpenTelemetry trace.
/// </para>
/// <para>
/// Expected domain failures (validation, not-found, conflict) should be handled by
/// returning <c>Result.Failure</c> from the handler and calling <c>Results.Problem</c>
/// in the endpoint — this handler is the last resort for truly unexpected exceptions.
/// </para>
/// </remarks>
internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>Initialises the handler with the structured logger.</summary>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    /// <returns><see langword="true"/> always, indicating the exception has been handled.</returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = "An unexpected error occurred. Please try again later.",
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
