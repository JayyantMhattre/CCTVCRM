namespace Ashraak.SharedKernel.Contracts.Webhooks;

/// <summary>Feature flag keys for the webhook platform capability.</summary>
public static class WebhookFeatureFlags
{
    /// <summary>Global / per-tenant gate for webhook APIs and publishing.</summary>
    public const string Enabled = "webhooks.enabled";
}
