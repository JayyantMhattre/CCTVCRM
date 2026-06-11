using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;
using Ashraak.Webhooks.Application.Mapping;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDeadLetter;
using Ashraak.Webhooks.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Webhooks.Infrastructure.Persistence.Repositories;

internal sealed class WebhookDeadLetterRepository(WebhooksDbContext context)
    : IWebhookDeadLetterStore, IWebhookDeadLetterRepository
{
    public Task<WebhookDeadLetter?> GetByIdAsync(
        WebhookDeadLetterId id,
        CancellationToken cancellationToken = default) =>
        context.WebhookDeadLetters
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task<IReadOnlyList<WebhookDeadLetter>> GetByTenantAsync(
        Guid tenantId,
        Guid? subscriptionId,
        string? eventName,
        DateTime? fromUtc,
        DateTime? toUtc,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var query = context.WebhookDeadLetters
            .Where(d => d.TenantId == tenantId)
            .AsQueryable();

        if (subscriptionId.HasValue)
            query = query.Where(d => d.SubscriptionId == subscriptionId.Value);

        if (!string.IsNullOrWhiteSpace(eventName))
            query = query.Where(d => d.EventName == eventName.Trim().ToLowerInvariant());

        if (fromUtc.HasValue)
            query = query.Where(d => d.CreatedOnUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(d => d.CreatedOnUtc <= toUtc.Value);

        return await query
            .OrderByDescending(d => d.CreatedOnUtc)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public void Add(WebhookDeadLetter deadLetter) => context.WebhookDeadLetters.Add(deadLetter);

    async Task<WebhookDeadLetterContract?> IWebhookDeadLetterRepository.GetByIdAsync(
        Guid tenantId,
        Guid deadLetterId,
        CancellationToken cancellationToken)
    {
        var entity = await context.WebhookDeadLetters
            .FirstOrDefaultAsync(
                d => d.Id == WebhookDeadLetterId.From(deadLetterId) && d.TenantId == tenantId,
                cancellationToken);

        return entity is null ? null : WebhookDeadLetterMapper.ToContract(entity);
    }

    async Task<IReadOnlyList<WebhookDeadLetterContract>> IWebhookDeadLetterRepository.GetByTenantAsync(
        Guid tenantId,
        Guid? subscriptionId,
        string? eventName,
        DateTime? fromUtc,
        DateTime? toUtc,
        int limit,
        CancellationToken cancellationToken)
    {
        var items = await GetByTenantAsync(
            tenantId,
            subscriptionId,
            eventName,
            fromUtc,
            toUtc,
            limit,
            cancellationToken);

        return items.Select(WebhookDeadLetterMapper.ToContract).ToList();
    }
}
