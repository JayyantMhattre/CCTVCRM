using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;

namespace Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;

public interface IWebhookDeadLetterRepository
{
    Task<WebhookDeadLetterContract?> GetByIdAsync(
        Guid tenantId,
        Guid deadLetterId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WebhookDeadLetterContract>> GetByTenantAsync(
        Guid tenantId,
        Guid? subscriptionId,
        string? eventName,
        DateTime? fromUtc,
        DateTime? toUtc,
        int limit,
        CancellationToken cancellationToken = default);
}
