using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Webhooks.Events;

/// <summary>
/// Raised when a webhook event has been accepted into the platform outbox for future delivery.
/// Distinct from subscription lifecycle domain events in the Webhooks module.
/// </summary>
public sealed record WebhookPublishedDomainEvent(
    Guid TenantId,
    string EventName,
    string Version,
    string? CorrelationId) : DomainEvent;
