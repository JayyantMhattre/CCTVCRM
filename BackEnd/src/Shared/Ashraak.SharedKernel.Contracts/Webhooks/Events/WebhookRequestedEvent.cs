using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Webhooks.Events;

/// <summary>
/// Enqueued to the webhooks outbox when a module publishes a webhook-eligible event.
/// W2 delivery engine will consume this; W1 only persists to outbox.
/// </summary>
public sealed record WebhookRequestedEvent(
    Guid TenantId,
    string EventName,
    string Version,
    string PayloadJson,
    string? CorrelationId) : DomainEvent;
