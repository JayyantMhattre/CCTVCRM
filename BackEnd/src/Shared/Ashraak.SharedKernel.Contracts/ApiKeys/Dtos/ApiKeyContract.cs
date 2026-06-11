namespace Ashraak.SharedKernel.Contracts.ApiKeys.Dtos;

/// <summary>Read model for an API key (never includes the secret).</summary>
public sealed record ApiKeyContract(
    Guid Id,
    Guid TenantId,
    string Name,
    string Description,
    string KeyPrefix,
    string Environment,
    IReadOnlyList<string> Scopes,
    Guid CreatedBy,
    DateTime CreatedOnUtc,
    DateTime? ExpiresOnUtc,
    DateTime? LastUsedOnUtc,
    DateTime? RevokedOnUtc,
    bool Enabled,
    long RequestCount,
    long SuccessCount,
    long FailureCount,
    string? LastCorrelationId);

/// <summary>Returned once on create or rotate — includes the full plaintext key.</summary>
public sealed record ApiKeyCreatedContract(
    Guid Id,
    Guid TenantId,
    string Name,
    string KeyPrefix,
    string PlaintextKey,
    IReadOnlyList<string> Scopes,
    DateTime CreatedOnUtc,
    DateTime? ExpiresOnUtc);
