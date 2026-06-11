using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Auth.Domain.ValueObjects;

/// <summary>
/// Value object representing a fine-grained ABAC permission.
/// Permissions follow the convention <c>"resource:action"</c> (e.g. <c>"tenant:read"</c>).
/// Pre-defined system permissions are exposed via the <see cref="Defaults"/> nested class
/// to prevent typos in calling code.
/// </summary>
/// <remarks>
/// Equality is based on the <see cref="Name"/> only; two permissions with identical names
/// but different descriptions are considered the same.
/// </remarks>
public sealed class Permission : ValueObject
{
    private Permission(string name, string description) { Name = name; Description = description; }

    /// <summary>Gets the machine-readable permission identifier (e.g. <c>"tenant:read"</c>).</summary>
    public string Name { get; }

    /// <summary>Gets a human-readable description of what this permission grants.</summary>
    public string Description { get; }

    /// <summary>
    /// Creates a new <see cref="Permission"/> with the given <paramref name="name"/> and <paramref name="description"/>.
    /// </summary>
    /// <param name="name">Permission identifier (e.g. <c>"user:invite"</c>).</param>
    /// <param name="description">Human-readable description.</param>
    public static Permission Create(string name, string description) => new(name, description);

    /// <summary>
    /// Well-known system permissions. Use these constants instead of raw strings
    /// to ensure consistent permission naming across the codebase.
    /// </summary>
    public static class Defaults
    {
        /// <summary>Allows reading tenant information and settings.</summary>
        public static readonly Permission TenantRead = Create("tenant:read", "Read tenant information");

        /// <summary>Allows modifying tenant settings and subscription.</summary>
        public static readonly Permission TenantWrite = Create("tenant:write", "Modify tenant settings");

        /// <summary>Allows reading user profile data within the tenant.</summary>
        public static readonly Permission UserRead = Create("user:read", "Read user profiles");

        /// <summary>Allows creating and updating user profiles within the tenant.</summary>
        public static readonly Permission UserWrite = Create("user:write", "Create and modify users");

        /// <summary>Allows sending invitation emails to add new users to the tenant.</summary>
        public static readonly Permission UserInvite = Create("user:invite", "Invite users to tenant");

        /// <summary>Allows reading audit log entries for the tenant.</summary>
        public static readonly Permission AuditRead = Create("audit:read", "Read audit logs");

        /// <summary>Allows assigning and removing roles from other users within the tenant.</summary>
        public static readonly Permission RoleManage = Create("role:manage", "Assign and manage roles");

        /// <summary>Allows reading webhook subscriptions and configuration.</summary>
        public static readonly Permission WebhooksRead = Create("webhooks:read", "Read webhook subscriptions");

        /// <summary>Allows creating and managing webhook subscriptions and secrets.</summary>
        public static readonly Permission WebhooksManage = Create("webhooks:manage", "Manage webhook subscriptions");

        /// <summary>Allows reading API key metadata and usage.</summary>
        public static readonly Permission ApiKeysRead = Create("apikeys:read", "Read API keys");

        /// <summary>Allows creating, rotating, and revoking API keys.</summary>
        public static readonly Permission ApiKeysManage = Create("apikeys:manage", "Manage API keys");
    }

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetAtomicValues() { yield return Name; }
}
