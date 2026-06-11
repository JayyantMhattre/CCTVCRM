namespace Ashraak.SharedKernel.Results;

/// <summary>
/// Classifies an <see cref="Error"/> so that the API layer can map it to the
/// correct HTTP status code without pattern-matching on error codes.
/// </summary>
public enum ErrorType
{
    /// <summary>A generic operation failure that does not fit a more specific category.</summary>
    Failure,

    /// <summary>Input data did not pass validation rules (HTTP 422 / 400).</summary>
    Validation,

    /// <summary>The requested resource could not be found (HTTP 404).</summary>
    NotFound,

    /// <summary>A conflict with the current state of the resource (HTTP 409).</summary>
    Conflict,

    /// <summary>The caller is not authenticated (HTTP 401).</summary>
    Unauthorized,

    /// <summary>The caller is authenticated but lacks the required permission (HTTP 403).</summary>
    Forbidden,

    /// <summary>The resource existed but has been permanently deleted (HTTP 410).</summary>
    Gone,

    /// <summary>The caller has exceeded the allowed request rate (HTTP 429).</summary>
    TooManyRequests,

    /// <summary>An unhandled exception or a bug — should never reach the client in production (HTTP 500).</summary>
    Unexpected
}

/// <summary>
/// Immutable description of a domain or application error used in the railway-oriented
/// <see cref="Result"/> / <see cref="Result{TValue}"/> pattern.
/// </summary>
/// <param name="Code">
/// A dot-separated error code, e.g. <c>"Tenant.NotFound"</c>.
/// Used by API clients and logging to identify the error without parsing the message.
/// </param>
/// <param name="Description">
/// A human-readable description of what went wrong.
/// Should be safe to expose to API consumers (no stack traces, no internal details).
/// </param>
/// <param name="Type">
/// The category of the error, used to map to HTTP status codes. Defaults to <see cref="ErrorType.Failure"/>.
/// </param>
public sealed record Error(string Code, string Description, ErrorType Type = ErrorType.Failure)
{
    /// <summary>Represents the absence of an error. Used internally by <see cref="Result.Success()"/>.</summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    /// <summary>Used when a required value is unexpectedly null.</summary>
    public static readonly Error NullValue = new("General.NullValue", "A null value was provided.", ErrorType.Unexpected);

    /// <summary>Creates a <see cref="ErrorType.NotFound"/> error.</summary>
    /// <param name="code">Machine-readable error code.</param>
    /// <param name="description">Human-readable description.</param>
    public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);

    /// <summary>Creates a <see cref="ErrorType.Validation"/> error.</summary>
    /// <param name="code">Machine-readable error code.</param>
    /// <param name="description">Human-readable description.</param>
    public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);

    /// <summary>Creates a <see cref="ErrorType.Conflict"/> error.</summary>
    /// <param name="code">Machine-readable error code.</param>
    /// <param name="description">Human-readable description.</param>
    public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);

    /// <summary>Creates a <see cref="ErrorType.Unauthorized"/> error.</summary>
    /// <param name="code">Machine-readable error code.</param>
    /// <param name="description">Human-readable description.</param>
    public static Error Unauthorized(string code, string description) => new(code, description, ErrorType.Unauthorized);

    /// <summary>Creates a <see cref="ErrorType.Forbidden"/> error.</summary>
    /// <param name="code">Machine-readable error code.</param>
    /// <param name="description">Human-readable description.</param>
    public static Error Forbidden(string code, string description) => new(code, description, ErrorType.Forbidden);

    /// <summary>Creates a <see cref="ErrorType.Unexpected"/> error.</summary>
    /// <param name="code">Machine-readable error code.</param>
    /// <param name="description">Human-readable description.</param>
    public static Error Unexpected(string code, string description) => new(code, description, ErrorType.Unexpected);
}
