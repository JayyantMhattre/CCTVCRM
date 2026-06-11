using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.BuildingBlocks.Application.Commands;

/// <summary>
/// Marker interface for a CQRS command that produces no value on success.
/// Implement this on command records whose handler performs a state change and
/// returns only a success/failure indication.
/// </summary>
/// <remarks>
/// Extending <see cref="IRequest{TResponse}"/> with <see cref="Result"/> wires the command
/// into the MediatR pipeline with the <c>ValidationBehavior</c>, <c>LoggingBehavior</c>,
/// and <c>PerformanceBehavior</c> pipeline behaviors automatically.
/// </remarks>
/// <example>
/// <code>
/// public sealed record DeleteTenantCommand(Guid TenantId) : ICommand;
/// </code>
/// </example>
public interface ICommand : IRequest<Result>;

/// <summary>
/// Marker interface for a CQRS command that produces a typed value on success.
/// Use this when the command creates a new resource and the caller needs its identifier.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the value returned on success
/// (e.g. <see cref="Guid"/> for a newly created entity's identifier).
/// </typeparam>
/// <example>
/// <code>
/// public sealed record RegisterUserCommand(...) : ICommand&lt;Guid&gt;;
/// </code>
/// </example>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
