namespace Ashraak.Audit.Domain.Entities;

/// <summary>
/// MongoDB document that captures a single auditable action.
/// Uses SHA-256 hash chaining to create a tamper-evident ledger — each entry's
/// <see cref="Hash"/> incorporates the <see cref="PreviousHash"/> of the preceding
/// entry for the same tenant, making retroactive modification detectable.
/// </summary>
/// <remarks>
/// All properties are <c>private init</c> to enforce creation through the <see cref="Create"/> factory.
/// The private parameterless constructor is required by the MongoDB BSON deserialiser.
/// </remarks>
public sealed class AuditEntry
{
    private AuditEntry() { }

    /// <summary>Gets the unique identifier of this audit record (compact GUID, no hyphens).</summary>
    public string Id { get; private init; } = string.Empty;

    /// <summary>Gets the tenant in whose context the audited action occurred.</summary>
    public Guid TenantId { get; private init; }

    /// <summary>
    /// Gets the user who performed the action, or <see langword="null"/> for system-initiated actions
    /// (background jobs, outbox processors).
    /// </summary>
    public Guid? UserId { get; private init; }

    /// <summary>Gets the source module name (e.g. <c>"Tenant"</c>, <c>"Auth"</c>).</summary>
    public string Module { get; private init; } = string.Empty;

    /// <summary>Gets the action verb describing what happened (e.g. <c>"Created"</c>, <c>"Suspended"</c>).</summary>
    public string Action { get; private init; } = string.Empty;

    /// <summary>Gets the aggregate or entity type affected (e.g. <c>"Tenant"</c>, <c>"UserProfile"</c>).</summary>
    public string EntityType { get; private init; } = string.Empty;

    /// <summary>Gets the string identifier of the affected entity, or <see langword="null"/> for non-entity events.</summary>
    public string? EntityId { get; private init; }

    /// <summary>Gets the JSON snapshot of the entity state before the change (null for create operations).</summary>
    public string? OldValues { get; private init; }

    /// <summary>Gets the JSON snapshot of the entity state after the change (null for delete operations).</summary>
    public string? NewValues { get; private init; }

    /// <summary>Gets the client IP address, if captured from the HTTP context.</summary>
    public string? IpAddress { get; private init; }

    /// <summary>Gets the client User-Agent header, if captured from the HTTP context.</summary>
    public string? UserAgent { get; private init; }

    /// <summary>Gets the UTC timestamp of when the domain event was raised.</summary>
    public DateTime OccurredOnUtc { get; private init; }

    /// <summary>
    /// Gets the <see cref="Hash"/> of the previous audit entry for this tenant.
    /// <see langword="null"/> for the first entry in the chain.
    /// Used to detect chain breaks during integrity validation.
    /// </summary>
    public string? PreviousHash { get; private init; }

    /// <summary>
    /// Gets the SHA-256 hash of this entry's key fields combined with <see cref="PreviousHash"/>.
    /// Written by <c>AuditRepository.LogAsync</c> after the entry is constructed.
    /// </summary>
    public string? Hash { get; private set; }

    /// <summary>
    /// Factory method that creates a new, unpersisted <see cref="AuditEntry"/>.
    /// The caller is responsible for computing and setting <see cref="Hash"/> via <see cref="SetHash"/>
    /// before inserting into MongoDB.
    /// </summary>
    public static AuditEntry Create(
        Guid tenantId,
        Guid? userId,
        string module,
        string action,
        string entityType,
        string? entityId,
        string? oldValues,
        string? newValues,
        string? ipAddress,
        string? userAgent,
        string? previousHash = null) => new()
        {
            Id = Guid.NewGuid().ToString("N"),
            TenantId = tenantId,
            UserId = userId,
            Module = module,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            OccurredOnUtc = DateTime.UtcNow,
            PreviousHash = previousHash
        };

    /// <summary>
    /// Sets the chain hash for this entry. Called once by <c>AuditRepository.LogAsync</c>
    /// immediately before inserting the document into MongoDB.
    /// </summary>
    /// <param name="hash">The hex-encoded SHA-256 hash.</param>
    public void SetHash(string hash) => Hash = hash;
}
