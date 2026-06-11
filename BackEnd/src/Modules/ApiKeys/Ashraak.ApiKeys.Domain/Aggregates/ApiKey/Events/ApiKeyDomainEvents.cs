using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.ApiKeys.Domain.Aggregates.ApiKey.Events;

public sealed record ApiKeyCreatedDomainEvent(
    Guid ApiKeyId,
    Guid TenantId,
    Guid CreatedBy,
    string Name,
    string KeyPrefix) : DomainEvent;

public sealed record ApiKeyRotatedDomainEvent(
    Guid ApiKeyId,
    Guid TenantId,
    Guid RotatedBy,
    string KeyPrefix) : DomainEvent;

public sealed record ApiKeyRevokedDomainEvent(
    Guid ApiKeyId,
    Guid TenantId,
    Guid RevokedBy) : DomainEvent;

public sealed record ApiKeyScopesChangedDomainEvent(
    Guid ApiKeyId,
    Guid TenantId,
    Guid ChangedBy,
    IReadOnlyList<string> Scopes) : DomainEvent;

public sealed record ApiKeyUsedDomainEvent(
    Guid ApiKeyId,
    Guid TenantId,
    string CorrelationId,
    bool Success) : DomainEvent;
