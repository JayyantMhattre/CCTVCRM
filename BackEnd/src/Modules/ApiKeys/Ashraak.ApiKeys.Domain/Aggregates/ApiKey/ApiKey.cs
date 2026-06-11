using Ashraak.ApiKeys.Domain.Aggregates.ApiKey.Events;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.ApiKeys.Domain.Aggregates.ApiKey;

/// <summary>Tenant-scoped machine-to-machine API key.</summary>
public sealed class ApiKey : AggregateRoot<ApiKeyId>
{
    private ApiKey(ApiKeyId id) : base(id) { }

    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string KeyPrefix { get; private set; } = string.Empty;
    public string HashedSecret { get; private set; } = string.Empty;
    public string Environment { get; private set; } = "prod";
    public List<string> Scopes { get; private set; } = [];
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ExpiresOnUtc { get; private set; }
    public DateTime? LastUsedOnUtc { get; private set; }
    public DateTime? RevokedOnUtc { get; private set; }
    public bool Enabled { get; private set; }
    public long RequestCount { get; private set; }
    public long SuccessCount { get; private set; }
    public long FailureCount { get; private set; }
    public string? LastCorrelationId { get; private set; }

    public bool IsActive =>
        Enabled &&
        RevokedOnUtc is null &&
        (ExpiresOnUtc is null || ExpiresOnUtc > DateTime.UtcNow);

    public static ApiKey Create(
        ApiKeyId id,
        Guid tenantId,
        string name,
        string description,
        string keyPrefix,
        string hashedSecret,
        string environment,
        IReadOnlyList<string> scopes,
        Guid createdBy,
        DateTime? expiresOnUtc)
    {
        var now = DateTime.UtcNow;
        var apiKey = new ApiKey(id)
        {
            TenantId = tenantId,
            Name = name.Trim(),
            Description = description.Trim(),
            KeyPrefix = keyPrefix,
            HashedSecret = hashedSecret,
            Environment = environment,
            Scopes = NormalizeScopes(scopes),
            CreatedBy = createdBy,
            CreatedOnUtc = now,
            ExpiresOnUtc = expiresOnUtc,
            Enabled = true
        };

        apiKey.RaiseDomainEvent(new ApiKeyCreatedDomainEvent(
            id.Value, tenantId, createdBy, apiKey.Name, apiKey.KeyPrefix));

        return apiKey;
    }

    public void Rotate(string keyPrefix, string hashedSecret, Guid rotatedBy)
    {
        KeyPrefix = keyPrefix;
        HashedSecret = hashedSecret;
        RevokedOnUtc = null;
        Enabled = true;

        RaiseDomainEvent(new ApiKeyRotatedDomainEvent(
            Id.Value, TenantId, rotatedBy, KeyPrefix));
    }

    public void Revoke(Guid revokedBy)
    {
        if (RevokedOnUtc is not null)
            return;

        Enabled = false;
        RevokedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new ApiKeyRevokedDomainEvent(Id.Value, TenantId, revokedBy));
    }

    public void UpdateScopes(IReadOnlyList<string> scopes, Guid changedBy)
    {
        Scopes = NormalizeScopes(scopes);

        RaiseDomainEvent(new ApiKeyScopesChangedDomainEvent(
            Id.Value, TenantId, changedBy, Scopes));
    }

    public void RecordUsage(string correlationId, bool success)
    {
        RequestCount++;
        if (success)
            SuccessCount++;
        else
            FailureCount++;

        LastUsedOnUtc = DateTime.UtcNow;
        LastCorrelationId = correlationId;

        RaiseDomainEvent(new ApiKeyUsedDomainEvent(
            Id.Value, TenantId, correlationId, success));
    }

    private static List<string> NormalizeScopes(IReadOnlyList<string>? scopes) =>
        scopes?
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim().ToLowerInvariant())
            .Distinct()
            .ToList() ?? [];
}
