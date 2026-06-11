using Ashraak.Webhooks.Domain.Aggregates.WebhookEventDefinition;

namespace Ashraak.Webhooks.Domain.Repositories;

public interface IWebhookEventDefinitionStore
{
    Task<WebhookEventDefinition?> GetByEventNameAsync(string eventName, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WebhookEventDefinition>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(WebhookEventDefinition definition);
}
