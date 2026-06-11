namespace Ashraak.Webhooks.Domain.Aggregates.WebhookDeadLetter;

public sealed record WebhookDeadLetterId(Guid Value)
{
    public static WebhookDeadLetterId New() => new(Guid.NewGuid());
    public static WebhookDeadLetterId From(Guid value) => new(value);
}
