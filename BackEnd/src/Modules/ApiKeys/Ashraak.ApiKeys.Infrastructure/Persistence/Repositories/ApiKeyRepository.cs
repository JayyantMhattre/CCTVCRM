using Ashraak.ApiKeys.Domain.Aggregates.ApiKey;
using Ashraak.ApiKeys.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.ApiKeys.Infrastructure.Persistence.Repositories;

internal sealed class ApiKeyRepository(ApiKeysDbContext db) : IApiKeyRepository
{
    public void Add(ApiKey apiKey) => db.ApiKeys.Add(apiKey);

    public Task<ApiKey?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken) =>
        db.ApiKeys.FirstOrDefaultAsync(k => k.TenantId == tenantId && k.Id == ApiKeyId.From(id), cancellationToken);

    public Task<ApiKey?> GetByPrefixAsync(string keyPrefix, CancellationToken cancellationToken) =>
        db.ApiKeys.IgnoreQueryFilters()
            .FirstOrDefaultAsync(k => k.KeyPrefix == keyPrefix, cancellationToken);

    public async Task<IReadOnlyList<ApiKey>> ListAsync(Guid tenantId, CancellationToken cancellationToken) =>
        await db.ApiKeys
            .Where(k => k.TenantId == tenantId)
            .OrderByDescending(k => k.CreatedOnUtc)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken) =>
        db.ApiKeys.AnyAsync(k => k.TenantId == tenantId && k.Name == name, cancellationToken);
}
