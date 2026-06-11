using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Auth.Events;

/// <summary>
/// Raised on every successful authentication (password, SSO, refresh token).
/// Handled by the Audit module to record the login entry with IP and user agent,
/// supporting security dashboards and anomaly detection.
/// </summary>
/// <param name="UserId">The authenticated user's identifier.</param>
/// <param name="TenantId">The tenant context of the login.</param>
/// <param name="Email">The email used to authenticate.</param>
/// <param name="IpAddress">The client's IP address extracted from the HTTP request.</param>
/// <param name="UserAgent">The client's <c>User-Agent</c> header value.</param>
/// <param name="LoggedInOnUtc">UTC timestamp of the successful authentication.</param>
public sealed record UserLoggedInEvent(
    Guid UserId,
    Guid TenantId,
    string Email,
    string IpAddress,
    string UserAgent,
    DateTime LoggedInOnUtc) : DomainEvent;
