namespace Ashraak.Webhooks.Application;

/// <summary>Permission names for webhook administration (ABAC).</summary>
public static class WebhookPermissions
{
    public const string Read = "webhooks:read";
    public const string Manage = "webhooks:manage";
}
