using System.Security.Claims;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Ashraak.Api.Infrastructure;

/// <summary>
/// HTTP-context-backed implementation of <see cref="ICurrentUser"/>.
/// Reads identity claims from the JWT access token that ASP.NET Core's
/// authentication middleware has validated and placed on <c>HttpContext.User</c>.
/// </summary>
/// <remarks>
/// Registered as a scoped service so that a new instance is created per HTTP request.
/// Injected into command/query handlers to provide authenticated user context
/// without a direct dependency on <c>IHttpContextAccessor</c>.
/// </remarks>
internal sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>Initialises the service with the HTTP context accessor.</summary>
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    /// <inheritdoc/>
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc/>
    /// <remarks>Extracted from the <c>sub</c> / <see cref="ClaimTypes.NameIdentifier"/> claim.</remarks>
    public Guid UserId => Guid.TryParse(
        User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

    /// <inheritdoc/>
    /// <remarks>Extracted from the custom <c>tenant_id</c> JWT claim.</remarks>
    public Guid TenantId => Guid.TryParse(
        User?.FindFirstValue("tenant_id"), out var id) ? id : Guid.Empty;

    /// <inheritdoc/>
    /// <remarks>Extracted from the <see cref="ClaimTypes.Email"/> claim.</remarks>
    public string Email => User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    /// <inheritdoc/>
    /// <remarks>All <see cref="ClaimTypes.Role"/> claims for the current user.</remarks>
    public IReadOnlyList<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? [];

    /// <inheritdoc/>
    /// <remarks>All <c>"permission"</c> claims embedded in the JWT by OpenIddict at token issuance.</remarks>
    public IReadOnlyList<string> Permissions =>
        User?.FindAll("permission").Select(c => c.Value).ToList() ?? [];

    /// <inheritdoc/>
    public bool IsInRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public bool HasPermission(string permission) => Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
}
