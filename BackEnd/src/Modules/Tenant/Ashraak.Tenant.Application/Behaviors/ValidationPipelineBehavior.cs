using Ashraak.SharedKernel.Results;
using FluentValidation;
using MediatR;

namespace Ashraak.Tenant.Application.Behaviors;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var errors = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .Select(f => new Error(f.PropertyName, f.ErrorMessage, ErrorType.Validation))
            .ToArray();

        if (errors.Length > 0)
        {
            return CreateValidationResult<TResponse>(errors);
        }

        return await next();
    }

    private static TResult CreateValidationResult<TResult>(Error[] errors)
    {
        if (typeof(TResult) == typeof(Result))
            return (TResult)(object)Result.Failure(errors[0]);

        var resultType = typeof(TResult).GetGenericArguments()[0];
        var failureMethod = typeof(Result).GetMethod(nameof(Result.Failure))!.MakeGenericMethod(resultType);
        return (TResult)failureMethod.Invoke(null, [errors[0]])!;
    }
}
