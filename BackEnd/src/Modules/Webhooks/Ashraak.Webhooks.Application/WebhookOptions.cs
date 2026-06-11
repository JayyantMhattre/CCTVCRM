namespace Ashraak.Webhooks.Application;

public sealed class WebhookOptions
{
    public const string SectionName = "Webhooks";

    /// <summary>When true, subscription endpoint URLs must use HTTPS.</summary>
    public bool RequireHttpsEndpoints { get; set; } = true;
}
