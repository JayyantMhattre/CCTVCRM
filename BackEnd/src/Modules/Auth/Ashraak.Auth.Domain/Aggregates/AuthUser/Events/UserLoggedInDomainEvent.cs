using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Auth.Domain.Aggregates.AuthUser.Events;

/// <summary>
/// Raised by <see cref="AuthUser.RecordSuccessfulLogin"/> after a successful authentication.
/// Handled by the Audit module to persist a login audit entry with the client's IP and user agent.
/// Also triggers the integration event <c>UserLoggedInEvent</c> in <c>SharedKernel.Contracts</c>
/// for other interested modules.
/// </summary>
/// <param name="UserId">The authenticated user's identifier.</param>
/// <param name="TenantId">The tenant context of the login.</param>
/// <param name="Email">The email address used during authentication.</param>
/// <param name="IpAddress">Client IP extracted from the HTTP request.</param>
/// <param name="UserAgent">Client's <c>User-Agent</c> header value.</param>
public sealed record UserLoggedInDomainEvent(
    Guid UserId,
    Guid TenantId,
    string Email,
    string IpAddress,
    string UserAgent) : DomainEvent;
