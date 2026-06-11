using Ashraak.Webhooks.Domain.Aggregates.WebhookEventDefinition;
using Ashraak.Webhooks.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Webhooks.Infrastructure.Persistence.Repositories;

internal sealed class WebhookEventDefinitionRepository(WebhooksDbContext context) : IWebhookEventDefinitionStore
{
    public Task<WebhookEventDefinition?> GetByEventNameAsync(string eventName, CancellationToken cancellationToken = default) =>
        context.WebhookEventDefinitions
            .FirstOrDefaultAsync(e => e.EventName == eventName.Trim().ToLowerInvariant(), cancellationToken);

    public async Task<IReadOnlyList<WebhookEventDefinition>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.WebhookEventDefinitions.OrderBy(e => e.EventName).ToListAsync(cancellationToken);

    public void Add(WebhookEventDefinition definition) => context.WebhookEventDefinitions.Add(definition);
}
