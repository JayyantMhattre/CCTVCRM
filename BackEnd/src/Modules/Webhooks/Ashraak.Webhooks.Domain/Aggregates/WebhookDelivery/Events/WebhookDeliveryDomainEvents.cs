using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery.Events;

public sealed record WebhookDeliveryRequestedDomainEvent(
    Guid DeliveryId,
    Guid SubscriptionId,
    Guid TenantId,
    string EventName,
    string EventVersion,
    string? CorrelationId) : DomainEvent;

public sealed record WebhookDeliverySucceededDomainEvent(
    Guid DeliveryId,
    Guid SubscriptionId,
    Guid TenantId,
    string EventName,
    int ResponseCode,
    string? CorrelationId) : DomainEvent;

public sealed record WebhookDeliveryFailedDomainEvent(
    Guid DeliveryId,
    Guid SubscriptionId,
    Guid TenantId,
    string EventName,
    int? ResponseCode,
    string? ErrorMessage,
    string? CorrelationId) : DomainEvent;

public sealed record WebhookRetryScheduledDomainEvent(
    Guid DeliveryId,
    Guid SubscriptionId,
    Guid TenantId,
    string EventName,
    int AttemptNumber,
    int RetryCount,
    DateTime NextRetryOnUtc,
    int? FailureCode,
    string? FailureReason,
    string? CorrelationId) : DomainEvent;

public sealed record WebhookRetryAttemptedDomainEvent(
    Guid DeliveryId,
    Guid SubscriptionId,
    Guid TenantId,
    string EventName,
    int AttemptNumber,
    string? CorrelationId) : DomainEvent;

public sealed record WebhookRetrySucceededDomainEvent(
    Guid DeliveryId,
    Guid SubscriptionId,
    Guid TenantId,
    string EventName,
    int AttemptNumber,
    int ResponseCode,
    string? CorrelationId) : DomainEvent;

public sealed record WebhookRetryFailedDomainEvent(
    Guid DeliveryId,
    Guid SubscriptionId,
    Guid TenantId,
    string EventName,
    int AttemptNumber,
    int? ResponseCode,
    string? ErrorMessage,
    string? CorrelationId) : DomainEvent;
