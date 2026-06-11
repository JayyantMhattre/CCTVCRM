using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;

namespace Ashraak.Webhooks.Domain.Repositories;

/// <summary>Module-internal persistence for webhook subscriptions.</summary>
public interface IWebhookSubscriptionStore
{
    Task<WebhookSubscription?> GetByIdAsync(WebhookSubscriptionId id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameExcludingIdAsync(
        Guid tenantId,
        string name,
        WebhookSubscriptionId excludeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WebhookSubscription>> GetEnabledForEventAsync(
        Guid tenantId,
        string eventName,
        CancellationToken cancellationToken = default);

    void Add(WebhookSubscription subscription);

    void Update(WebhookSubscription subscription);
}
