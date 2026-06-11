using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.BuildingBlocks.Application.Commands;

/// <summary>
/// Typed <c>IRequestHandler</c> alias for commands that return no value.
/// Implementing this instead of the raw MediatR interface provides a more
/// intent-revealing type signature and participates in the same pipeline behaviors.
/// </summary>
/// <typeparam name="TCommand">The command type this handler processes. Must implement <see cref="ICommand"/>.</typeparam>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand;

/// <summary>
/// Typed <c>IRequestHandler</c> alias for commands that return a value.
/// </summary>
/// <typeparam name="TCommand">The command type. Must implement <see cref="ICommand{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the value returned on success.</typeparam>
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;
