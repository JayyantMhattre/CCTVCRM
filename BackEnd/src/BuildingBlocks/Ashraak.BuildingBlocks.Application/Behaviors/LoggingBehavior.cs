using MediatR;
using Serilog;

namespace Ashraak.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Logs every request name and its execution time via Serilog structured logging.
/// Captures exceptions and re-throws after logging.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>(ILogger logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.Information("Handling {RequestName}", requestName);

        try
        {
            var response = await next();
            logger.Information("Handled {RequestName}", requestName);
            return response;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error handling {RequestName}", requestName);
            throw;
        }
    }
}
