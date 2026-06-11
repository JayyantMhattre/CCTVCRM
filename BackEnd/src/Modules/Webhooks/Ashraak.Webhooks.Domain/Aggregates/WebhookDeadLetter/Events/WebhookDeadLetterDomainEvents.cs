using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Webhooks.Domain.Aggregates.WebhookDeadLetter.Events;

public sealed record WebhookDeadLetterCreatedDomainEvent(
    Guid DeadLetterId,
    Guid DeliveryId,
    Guid SubscriptionId,
    Guid TenantId,
    string EventName,
    int? FailureCode,
    string? FailureReason,
    string? CorrelationId) : DomainEvent;

public sealed record WebhookDeadLetterReplayedDomainEvent(
    Guid DeadLetterId,
    Guid OriginalDeliveryId,
    Guid NewDeliveryId,
    Guid SubscriptionId,
    Guid TenantId,
    string EventName,
    string? CorrelationId) : DomainEvent;
