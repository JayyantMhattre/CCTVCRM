using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Enums;

namespace Ashraak.Webhooks.Application.Mapping;

public static class WebhookDeliveryMapper
{
    public static WebhookDeliveryContract ToContract(WebhookDelivery delivery) =>
        new(
            delivery.Id.Value,
            delivery.SubscriptionId,
            delivery.TenantId,
            delivery.EventName,
            delivery.EventVersion,
            delivery.CorrelationId,
            delivery.AttemptNumber,
            delivery.RetryCount,
            delivery.Status.ToString(),
            delivery.ResponseCode,
            delivery.ResponseBody,
            delivery.LastFailureReason,
            delivery.LastFailureCode,
            delivery.NextRetryOnUtc,
            delivery.StartedOnUtc,
            delivery.CompletedOnUtc);

    public static WebhookDeliveryStatus? ParseStatus(string? status) =>
        string.IsNullOrWhiteSpace(status)
            ? null
            : Enum.TryParse<WebhookDeliveryStatus>(status, ignoreCase: true, out var parsed)
                ? parsed
                : null;
}
