using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDeadLetter;

namespace Ashraak.Webhooks.Application.Mapping;

public static class WebhookDeadLetterMapper
{
    public static WebhookDeadLetterContract ToContract(WebhookDeadLetter deadLetter) =>
        new(
            deadLetter.Id.Value,
            deadLetter.DeliveryId,
            deadLetter.SubscriptionId,
            deadLetter.TenantId,
            deadLetter.EventName,
            deadLetter.Payload,
            deadLetter.FailureReason,
            deadLetter.FailureCode,
            deadLetter.RetryCount,
            deadLetter.CorrelationId,
            deadLetter.CreatedOnUtc);
}
