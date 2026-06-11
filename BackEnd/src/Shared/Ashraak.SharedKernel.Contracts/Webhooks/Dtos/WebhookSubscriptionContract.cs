namespace Ashraak.SharedKernel.Contracts.Webhooks.Dtos;

/// <summary>Read model for a tenant webhook subscription (secret never included).</summary>
public sealed record WebhookSubscriptionContract(
    Guid Id,
    Guid TenantId,
    string Name,
    string EndpointUrl,
    bool Enabled,
    Guid CreatedBy,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc);

/// <summary>Response when a subscription is created or secret is rotated (secret shown once).</summary>
public sealed record WebhookSubscriptionSecretContract(
    Guid Id,
    Guid TenantId,
    string Name,
    string EndpointUrl,
    bool Enabled,
    string Secret,
    Guid CreatedBy,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc);
