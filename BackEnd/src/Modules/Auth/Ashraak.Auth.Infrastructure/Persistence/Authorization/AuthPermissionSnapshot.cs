namespace Ashraak.Auth.Infrastructure.Persistence.Authorization;

/// <summary>
/// Cached authorisation snapshot used by the permission checker and token issuer.
/// </summary>
/// <param name="Roles">Resolved RBAC role names for the user.</param>
/// <param name="Permissions">Resolved permission names after ABAC evaluation.</param>
internal sealed record AuthPermissionSnapshot(
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);
