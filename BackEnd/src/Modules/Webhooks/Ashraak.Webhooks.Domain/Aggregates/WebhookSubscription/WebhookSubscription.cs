using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription.Events;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;

/// <summary>Tenant-scoped outbound webhook subscription (endpoint + signing secret).</summary>
public sealed class WebhookSubscription : AggregateRoot<WebhookSubscriptionId>
{
    private WebhookSubscription(WebhookSubscriptionId id) : base(id) { }

    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string EndpointUrl { get; private set; } = string.Empty;
    public string SecretProtected { get; private set; } = string.Empty;
    public bool Enabled { get; private set; }
    /// <summary>Empty list means all catalog events; otherwise only listed event names are delivered.</summary>
    public List<string> SubscribedEventNames { get; private set; } = [];
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime UpdatedOnUtc { get; private set; }

    public static WebhookSubscription Create(
        WebhookSubscriptionId id,
        Guid tenantId,
        string name,
        string endpointUrl,
        string secretProtected,
        Guid createdBy,
        IReadOnlyList<string>? subscribedEventNames = null)
    {
        var now = DateTime.UtcNow;
        var subscription = new WebhookSubscription(id)
        {
            TenantId = tenantId,
            Name = name.Trim(),
            EndpointUrl = endpointUrl.Trim(),
            SecretProtected = secretProtected,
            Enabled = true,
            SubscribedEventNames = NormalizeEventNames(subscribedEventNames),
            CreatedBy = createdBy,
            CreatedOnUtc = now,
            UpdatedOnUtc = now
        };

        subscription.RaiseDomainEvent(new WebhookSubscriptionCreatedDomainEvent(
            id.Value,
            tenantId,
            createdBy,
            subscription.Name,
            subscription.EndpointUrl));

        return subscription;
    }

    public void Update(
        string name,
        string endpointUrl,
        bool enabled,
        Guid updatedBy,
        IReadOnlyList<string>? subscribedEventNames = null)
    {
        Name = name.Trim();
        EndpointUrl = endpointUrl.Trim();
        Enabled = enabled;
        if (subscribedEventNames is not null)
            SubscribedEventNames = NormalizeEventNames(subscribedEventNames);
        UpdatedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new WebhookSubscriptionUpdatedDomainEvent(
            Id.Value,
            TenantId,
            updatedBy,
            Name,
            EndpointUrl));
    }

    public void Disable(Guid disabledBy)
    {
        if (!Enabled)
            return;

        Enabled = false;
        UpdatedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new WebhookSubscriptionDisabledDomainEvent(
            Id.Value,
            TenantId,
            disabledBy));
    }

    public void RotateSecret(string secretProtected, Guid rotatedBy)
    {
        SecretProtected = secretProtected;
        UpdatedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new WebhookSecretRotatedDomainEvent(
            Id.Value,
            TenantId,
            rotatedBy));
    }

    public bool IsSubscribedToEvent(string eventName)
    {
        if (SubscribedEventNames.Count == 0)
            return true;

        return SubscribedEventNames.Contains(eventName.Trim().ToLowerInvariant(), StringComparer.Ordinal);
    }

    private static List<string> NormalizeEventNames(IReadOnlyList<string>? names) =>
        names is null || names.Count == 0
            ? []
            : names.Select(n => n.Trim().ToLowerInvariant()).Distinct().ToList();
}
