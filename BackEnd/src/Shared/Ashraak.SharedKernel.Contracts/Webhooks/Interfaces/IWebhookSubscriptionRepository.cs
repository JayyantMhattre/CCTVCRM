using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;

namespace Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;

/// <summary>Read access to tenant webhook subscriptions (cross-module safe).</summary>
public interface IWebhookSubscriptionRepository
{
    Task<WebhookSubscriptionContract?> GetByIdAsync(
        Guid tenantId,
        Guid subscriptionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WebhookSubscriptionContract>> GetByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
