using Ashraak.SharedKernel.Domain.Primitives;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDeadLetter.Events;

namespace Ashraak.Webhooks.Domain.Aggregates.WebhookDeadLetter;

/// <summary>Terminal failure record for exhausted webhook delivery retries.</summary>
public sealed class WebhookDeadLetter : AggregateRoot<WebhookDeadLetterId>
{
    private WebhookDeadLetter(WebhookDeadLetterId id) : base(id) { }

    public Guid DeliveryId { get; private set; }
    public Guid SubscriptionId { get; private set; }
    public Guid TenantId { get; private set; }
    public string EventName { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public string? FailureReason { get; private set; }
    public int? FailureCode { get; private set; }
    public int RetryCount { get; private set; }
    public string? CorrelationId { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

    public static WebhookDeadLetter Create(
        WebhookDeadLetterId id,
        Guid deliveryId,
        Guid subscriptionId,
        Guid tenantId,
        string eventName,
        string payload,
        string? failureReason,
        int? failureCode,
        int retryCount,
        string? correlationId)
    {
        var deadLetter = new WebhookDeadLetter(id)
        {
            DeliveryId = deliveryId,
            SubscriptionId = subscriptionId,
            TenantId = tenantId,
            EventName = eventName,
            Payload = payload,
            FailureReason = Truncate(failureReason),
            FailureCode = failureCode,
            RetryCount = retryCount,
            CorrelationId = correlationId,
            CreatedOnUtc = DateTime.UtcNow
        };

        deadLetter.RaiseDomainEvent(new WebhookDeadLetterCreatedDomainEvent(
            id.Value,
            deliveryId,
            subscriptionId,
            tenantId,
            eventName,
            failureCode,
            failureReason,
            correlationId));

        return deadLetter;
    }

    public void MarkReplayed(Guid newDeliveryId)
    {
        RaiseDomainEvent(new WebhookDeadLetterReplayedDomainEvent(
            Id.Value,
            DeliveryId,
            newDeliveryId,
            SubscriptionId,
            TenantId,
            EventName,
            CorrelationId));
    }

    private static string? Truncate(string? value, int maxLength = 500) =>
        value is null ? null : value.Length <= maxLength ? value : value[..maxLength];
}
