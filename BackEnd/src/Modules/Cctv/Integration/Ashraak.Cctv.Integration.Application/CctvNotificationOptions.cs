namespace Ashraak.Cctv.Integration.Application;

/// <summary>CCTV notification configuration (portal links, defaults).</summary>
public sealed class CctvNotificationOptions
{
    public const string SectionName = "Cctv:Notifications";

    public string PortalUrl { get; set; } = "http://localhost:5173";

    /// <summary>Fallback tenant for outbox/background notification dispatch.</summary>
    public Guid DefaultTenantId { get; set; }
}
