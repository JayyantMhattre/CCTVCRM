using Ashraak.Webhooks.Domain.Aggregates.WebhookDeadLetter;

namespace Ashraak.Webhooks.Domain.Repositories;

public interface IWebhookDeadLetterStore
{
    Task<WebhookDeadLetter?> GetByIdAsync(WebhookDeadLetterId id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WebhookDeadLetter>> GetByTenantAsync(
        Guid tenantId,
        Guid? subscriptionId,
        string? eventName,
        DateTime? fromUtc,
        DateTime? toUtc,
        int limit,
        CancellationToken cancellationToken = default);

    void Add(WebhookDeadLetter deadLetter);
}
