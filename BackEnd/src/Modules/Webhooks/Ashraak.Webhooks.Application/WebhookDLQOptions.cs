namespace Ashraak.Webhooks.Application;

public sealed class WebhookDLQOptions
{
    public const string SectionName = "WebhookDLQ";

    public bool Enabled { get; set; } = true;

    public int RetentionDays { get; set; } = 90;
}
