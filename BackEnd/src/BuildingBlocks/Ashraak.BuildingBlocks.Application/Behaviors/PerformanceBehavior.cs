using System.Diagnostics;
using MediatR;
using Serilog;

namespace Ashraak.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Warns when a request takes longer than the configured threshold (default 500 ms).
/// Helps surface N+1 queries and slow handlers early in development.
/// </summary>
public sealed class PerformanceBehavior<TRequest, TResponse>(ILogger logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const int SlowRequestThresholdMs = 500;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        if (sw.ElapsedMilliseconds > SlowRequestThresholdMs)
        {
            logger.Warning(
                "Slow request detected: {RequestName} took {ElapsedMs} ms",
                typeof(TRequest).Name,
                sw.ElapsedMilliseconds);
        }

        return response;
    }
}
