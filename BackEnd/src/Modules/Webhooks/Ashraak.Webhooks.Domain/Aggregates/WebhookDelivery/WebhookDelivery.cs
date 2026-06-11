using Ashraak.SharedKernel.Domain.Primitives;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery.Events;
using Ashraak.Webhooks.Domain.Enums;

namespace Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;

/// <summary>HTTP delivery attempt for a webhook event to a subscription endpoint.</summary>
public sealed class WebhookDelivery : AggregateRoot<WebhookDeliveryId>
{
    private WebhookDelivery(WebhookDeliveryId id) : base(id) { }

    public Guid SubscriptionId { get; private set; }
    public Guid TenantId { get; private set; }
    public string EventName { get; private set; } = string.Empty;
    public string EventVersion { get; private set; } = string.Empty;
    public string? CorrelationId { get; private set; }
    public string Payload { get; private set; } = string.Empty;
    public int AttemptNumber { get; private set; }
    public int RetryCount { get; private set; }
    public WebhookDeliveryStatus Status { get; private set; }
    public int? ResponseCode { get; private set; }
    public string? ResponseBody { get; private set; }
    public string? LastFailureReason { get; private set; }
    public int? LastFailureCode { get; private set; }
    public DateTime? NextRetryOnUtc { get; private set; }
    public DateTime StartedOnUtc { get; private set; }
    public DateTime? CompletedOnUtc { get; private set; }

    public static WebhookDelivery CreatePending(
        WebhookDeliveryId id,
        Guid subscriptionId,
        Guid tenantId,
        string eventName,
        string eventVersion,
        string? correlationId,
        string payload)
    {
        var delivery = new WebhookDelivery(id)
        {
            SubscriptionId = subscriptionId,
            TenantId = tenantId,
            EventName = eventName,
            EventVersion = eventVersion,
            CorrelationId = correlationId,
            Payload = payload,
            AttemptNumber = 1,
            RetryCount = 0,
            Status = WebhookDeliveryStatus.Pending,
            StartedOnUtc = DateTime.UtcNow
        };

        delivery.RaiseDomainEvent(new WebhookDeliveryRequestedDomainEvent(
            id.Value,
            subscriptionId,
            tenantId,
            eventName,
            eventVersion,
            correlationId));

        return delivery;
    }

    public void MarkSucceeded(int responseCode, string? responseBody)
    {
        Status = WebhookDeliveryStatus.Succeeded;
        ResponseCode = responseCode;
        ResponseBody = Truncate(responseBody);
        CompletedOnUtc = DateTime.UtcNow;
        NextRetryOnUtc = null;

        if (AttemptNumber > 1)
        {
            RaiseDomainEvent(new WebhookRetrySucceededDomainEvent(
                Id.Value,
                SubscriptionId,
                TenantId,
                EventName,
                AttemptNumber,
                responseCode,
                CorrelationId));
        }
        else
        {
            RaiseDomainEvent(new WebhookDeliverySucceededDomainEvent(
                Id.Value,
                SubscriptionId,
                TenantId,
                EventName,
                responseCode,
                CorrelationId));
        }
    }

    public void MarkFailed(int? responseCode, string? responseBody, string? errorMessage)
    {
        Status = WebhookDeliveryStatus.Failed;
        ResponseCode = responseCode;
        ResponseBody = Truncate(responseBody ?? errorMessage);
        LastFailureCode = responseCode;
        LastFailureReason = Truncate(errorMessage, 500);
        CompletedOnUtc = DateTime.UtcNow;
        NextRetryOnUtc = null;

        RaiseDomainEvent(new WebhookDeliveryFailedDomainEvent(
            Id.Value,
            SubscriptionId,
            TenantId,
            EventName,
            responseCode,
            errorMessage,
            CorrelationId));
    }

    public void ScheduleRetry(DateTime nextRetryOnUtc, int? failureCode, string? failureReason)
    {
        RetryCount++;
        LastFailureCode = failureCode;
        LastFailureReason = Truncate(failureReason, 500);
        ResponseCode = failureCode;
        NextRetryOnUtc = nextRetryOnUtc;
        Status = WebhookDeliveryStatus.Retrying;
        CompletedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new WebhookRetryFailedDomainEvent(
            Id.Value,
            SubscriptionId,
            TenantId,
            EventName,
            AttemptNumber,
            failureCode,
            failureReason,
            CorrelationId));

        RaiseDomainEvent(new WebhookRetryScheduledDomainEvent(
            Id.Value,
            SubscriptionId,
            TenantId,
            EventName,
            AttemptNumber,
            RetryCount,
            nextRetryOnUtc,
            failureCode,
            failureReason,
            CorrelationId));
    }

    public void BeginRetryAttempt()
    {
        AttemptNumber++;
        Status = WebhookDeliveryStatus.Pending;
        NextRetryOnUtc = null;
        CompletedOnUtc = null;

        RaiseDomainEvent(new WebhookRetryAttemptedDomainEvent(
            Id.Value,
            SubscriptionId,
            TenantId,
            EventName,
            AttemptNumber,
            CorrelationId));
    }

    public void MarkDeadLettered(int? failureCode, string? failureReason)
    {
        Status = WebhookDeliveryStatus.DeadLettered;
        LastFailureCode = failureCode;
        LastFailureReason = Truncate(failureReason, 500);
        CompletedOnUtc = DateTime.UtcNow;
        NextRetryOnUtc = null;
    }

    public void PrepareManualRetry()
    {
        Status = WebhookDeliveryStatus.Pending;
        NextRetryOnUtc = null;
        CompletedOnUtc = null;
        AttemptNumber++;
    }

    private static string? Truncate(string? value, int maxLength = 4000) =>
        value is null ? null : value.Length <= maxLength ? value : value[..maxLength];
}
