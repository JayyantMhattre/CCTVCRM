namespace Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;

public sealed record WebhookDeliveryId(Guid Value)
{
    public static WebhookDeliveryId New() => new(Guid.NewGuid());
    public static WebhookDeliveryId From(Guid value) => new(value);
}
