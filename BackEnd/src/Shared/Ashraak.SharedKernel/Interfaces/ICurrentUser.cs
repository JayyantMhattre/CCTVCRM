namespace Ashraak.SharedKernel.Interfaces;

/// <summary>
/// Provides information about the authenticated user making the current HTTP request.
/// Implemented by <c>CurrentUser</c> in the Host API and backed by the JWT claims principal.
/// </summary>
/// <remarks>
/// Injected as a scoped service. Handlers, repositories, and services receive this via
/// constructor injection instead of accessing <c>HttpContext</c> directly, keeping them
/// framework-agnostic and testable.
/// </remarks>
public interface ICurrentUser
{
    /// <summary>Gets the unique identifier of the authenticated user.</summary>
    Guid UserId { get; }

    /// <summary>Gets the tenant the authenticated user belongs to.</summary>
    Guid TenantId { get; }

    /// <summary>Gets the email address extracted from the identity token.</summary>
    string Email { get; }

    /// <summary>Gets all roles assigned to the current user (RBAC).</summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>
    /// Gets all fine-grained permissions assigned to the current user (ABAC).
    /// Permissions follow the pattern <c>"Module.Resource.Action"</c>,
    /// e.g. <c>"Tenant.Subscription.Upgrade"</c>.
    /// </summary>
    IReadOnlyList<string> Permissions { get; }

    /// <summary>
    /// Gets a value indicating whether the current request carries a valid authenticated identity.
    /// Returns <see langword="false"/> for anonymous or expired tokens.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Returns <see langword="true"/> when the current user has been assigned the given <paramref name="role"/>.
    /// </summary>
    /// <param name="role">Role name to check (case-sensitive).</param>
    bool IsInRole(string role);

    /// <summary>
    /// Returns <see langword="true"/> when the current user holds the specified fine-grained <paramref name="permission"/>.
    /// </summary>
    /// <param name="permission">Permission string in <c>"Module.Resource.Action"</c> format.</param>
    bool HasPermission(string permission);
}
