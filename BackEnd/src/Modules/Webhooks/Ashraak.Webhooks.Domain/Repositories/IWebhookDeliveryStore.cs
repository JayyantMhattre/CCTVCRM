using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Enums;

namespace Ashraak.Webhooks.Domain.Repositories;

public interface IWebhookDeliveryStore
{
    Task<WebhookDelivery?> GetByIdAsync(WebhookDeliveryId id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WebhookDelivery>> GetHistoryAsync(
        Guid tenantId,
        Guid? subscriptionId,
        string? eventName,
        WebhookDeliveryStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        int limit,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WebhookDelivery>> GetDueForRetryAsync(
        DateTime utcNow,
        int batchSize,
        CancellationToken cancellationToken = default);

    Task<WebhookDelivery?> GetByIdForTenantAsync(
        WebhookDeliveryId id,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    void Add(WebhookDelivery delivery);

    void Update(WebhookDelivery delivery);
}
