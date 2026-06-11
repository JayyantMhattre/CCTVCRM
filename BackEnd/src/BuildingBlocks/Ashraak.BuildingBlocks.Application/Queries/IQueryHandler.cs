using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.BuildingBlocks.Application.Queries;

/// <summary>
/// Typed <c>IRequestHandler</c> alias for query handlers.
/// Implementing this interface instead of the raw MediatR interface ensures that
/// the handler participates in the <c>LoggingBehavior</c> and <c>PerformanceBehavior</c>
/// pipeline behaviors.
/// </summary>
/// <typeparam name="TQuery">The query type. Must implement <see cref="IQuery{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type returned on success.</typeparam>
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
