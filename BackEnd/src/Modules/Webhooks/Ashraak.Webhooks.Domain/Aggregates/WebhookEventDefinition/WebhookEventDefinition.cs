using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Webhooks.Domain.Aggregates.WebhookEventDefinition;

/// <summary>Catalog entry for a webhook-eligible platform event (global registry).</summary>
public sealed class WebhookEventDefinition : Entity<Guid>
{
    private WebhookEventDefinition(Guid id) : base(id) { }

    public string EventName { get; private set; } = string.Empty;
    public string Version { get; private set; } = "v1";
    public string Description { get; private set; } = string.Empty;
    public bool Enabled { get; private set; }

    public static WebhookEventDefinition Create(
        Guid id,
        string eventName,
        string version,
        string description,
        bool enabled = true)
    {
        return new WebhookEventDefinition(id)
        {
            EventName = eventName.Trim().ToLowerInvariant(),
            Version = version.Trim().ToLowerInvariant(),
            Description = description.Trim(),
            Enabled = enabled
        };
    }

    public static WebhookEventDefinition Seed(string eventName, string description) =>
        Create(Guid.NewGuid(), eventName, "v1", description);
}
