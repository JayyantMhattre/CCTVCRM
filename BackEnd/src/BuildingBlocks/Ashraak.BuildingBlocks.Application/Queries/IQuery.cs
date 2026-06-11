using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.BuildingBlocks.Application.Queries;

/// <summary>
/// Marker interface for a CQRS query that returns a typed result.
/// Query objects should be pure data — no side effects, no state changes.
/// Handlers read from the database (or cache) and return a <see cref="Result{TResponse}"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the value returned on success (typically a DTO or paged list).</typeparam>
/// <example>
/// <code>
/// public sealed record GetTenantQuery(Guid TenantId) : IQuery&lt;TenantDto&gt;;
/// </code>
/// </example>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
