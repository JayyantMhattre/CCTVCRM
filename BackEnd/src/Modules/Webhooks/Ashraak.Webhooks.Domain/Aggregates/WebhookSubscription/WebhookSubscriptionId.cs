namespace Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;

public sealed record WebhookSubscriptionId(Guid Value)
{
    public static WebhookSubscriptionId New() => new(Guid.NewGuid());
    public static WebhookSubscriptionId From(Guid value) => new(value);
}
