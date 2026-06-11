using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;

namespace Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;

public interface IWebhookDeliveryRepository
{
    Task<WebhookDeliveryContract?> GetByIdAsync(
        Guid tenantId,
        Guid deliveryId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WebhookDeliveryContract>> GetHistoryAsync(
        Guid tenantId,
        Guid? subscriptionId,
        string? eventName,
        string? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        int limit,
        CancellationToken cancellationToken = default);
}
