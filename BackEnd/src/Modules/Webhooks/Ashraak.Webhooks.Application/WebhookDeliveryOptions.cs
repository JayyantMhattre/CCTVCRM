namespace Ashraak.Webhooks.Application;

public sealed class WebhookDeliveryOptions
{
    public const string SectionName = "WebhookDelivery";

    public int TimeoutSeconds { get; set; } = 30;
    public int MaxConcurrentDeliveries { get; set; } = 4;
    public string UserAgent { get; set; } = "Ashraak-Webhooks/1.0";
}
