namespace Ashraak.SharedKernel.Results;

/// <summary>
/// Railway-oriented result type. Represents either a successful operation (no value)
/// or a failure with a descriptive <see cref="Error"/>.
/// </summary>
/// <remarks>
/// <para>
/// Use <see cref="Result"/> for commands that do not return a value.
/// Use <see cref="Result{TValue}"/> for queries or commands that produce output.
/// </para>
/// <para>
/// The railway pattern eliminates exceptions for expected failure cases, forces
/// callers to handle both outcomes, and maps cleanly to HTTP problem details in the
/// API layer.
/// </para>
/// <example>
/// <code>
/// var result = await _mediator.Send(command);
/// return result.IsSuccess ? Results.NoContent() : result.ToProblem();
/// </code>
/// </example>
/// </remarks>
public class Result
{
    /// <summary>
    /// Initialises a result. Enforces the invariant that a success cannot carry an error
    /// and a failure must have one.
    /// </summary>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="error">The error (must be <see cref="Error.None"/> on success).</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="isSuccess"/> and <paramref name="error"/> are inconsistent.
    /// </exception>
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Success result cannot have an error.");
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets a value indicating whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error associated with the failure.
    /// Will be <see cref="Error.None"/> on a successful result.
    /// </summary>
    public Error Error { get; }

    /// <summary>Creates a successful result with no value.</summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>Creates a failed result carrying the given <paramref name="error"/>.</summary>
    /// <param name="error">The error describing the failure.</param>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>Creates a successful result carrying <paramref name="value"/>.</summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    /// <param name="value">The value to wrap.</param>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    /// <summary>Creates a typed failed result carrying the given <paramref name="error"/>.</summary>
    /// <typeparam name="TValue">The expected value type (not used on failure).</typeparam>
    /// <param name="error">The error describing the failure.</param>
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    /// <summary>
    /// Allows returning an <see cref="Error"/> directly from a method whose return type is
    /// <see cref="Result"/>. The error is wrapped in a failed result automatically.
    /// </summary>
    public static implicit operator Result(Error error) => Failure(error);
}

/// <summary>
/// Railway-oriented result type that carries a value on success.
/// </summary>
/// <typeparam name="TValue">
/// The type of the value returned on success (e.g. a DTO, an entity Id).
/// </typeparam>
public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// Internal constructor called by the factory methods on <see cref="Result"/>.
    /// </summary>
    internal Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value of a successful result.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the result represents a failure. Always check <see cref="Result.IsSuccess"/>
    /// before accessing this property.
    /// </exception>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result.");

    /// <summary>
    /// Wraps a non-null value in a successful result.
    /// Wraps a <see langword="null"/> value in a failed result with <see cref="Error.NullValue"/>.
    /// Enables concise command handler returns: <c>return entity;</c>
    /// </summary>
    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    /// <summary>
    /// Allows returning an <see cref="Error"/> directly from a method whose return type is
    /// <see cref="Result{TValue}"/>. The error is wrapped in a typed failed result.
    /// </summary>
    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);

    /// <summary>
    /// Projects the value of a successful result through <paramref name="mapper"/>.
    /// Passes failures through unchanged.
    /// </summary>
    /// <typeparam name="TNew">The target type after mapping.</typeparam>
    /// <param name="mapper">Transformation function applied to the value.</param>
    public Result<TNew> Map<TNew>(Func<TValue, TNew> mapper) =>
        IsSuccess ? Result.Success(mapper(Value)) : Result.Failure<TNew>(Error);

    /// <summary>
    /// Asynchronous variant of <see cref="Map{TNew}(Func{TValue, TNew})"/>.
    /// </summary>
    /// <typeparam name="TNew">The target type after mapping.</typeparam>
    /// <param name="mapper">Async transformation function.</param>
    public async Task<Result<TNew>> MapAsync<TNew>(Func<TValue, Task<TNew>> mapper) =>
        IsSuccess ? Result.Success(await mapper(Value)) : Result.Failure<TNew>(Error);
}
