using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;

namespace Ashraak.Webhooks.Application.Mapping;

public static class WebhookSubscriptionMapper
{
    public static WebhookSubscriptionContract ToContract(WebhookSubscription subscription) =>
        new(
            subscription.Id.Value,
            subscription.TenantId,
            subscription.Name,
            subscription.EndpointUrl,
            subscription.Enabled,
            subscription.CreatedBy,
            subscription.CreatedOnUtc,
            subscription.UpdatedOnUtc);
}
