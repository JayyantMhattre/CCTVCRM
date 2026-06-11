using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription.Events;

public sealed record WebhookSubscriptionCreatedDomainEvent(
    Guid SubscriptionId,
    Guid TenantId,
    Guid CreatedBy,
    string Name,
    string EndpointUrl) : DomainEvent;

public sealed record WebhookSubscriptionUpdatedDomainEvent(
    Guid SubscriptionId,
    Guid TenantId,
    Guid UpdatedBy,
    string Name,
    string EndpointUrl) : DomainEvent;

public sealed record WebhookSubscriptionDisabledDomainEvent(
    Guid SubscriptionId,
    Guid TenantId,
    Guid DisabledBy) : DomainEvent;

public sealed record WebhookSecretRotatedDomainEvent(
    Guid SubscriptionId,
    Guid TenantId,
    Guid RotatedBy) : DomainEvent;
