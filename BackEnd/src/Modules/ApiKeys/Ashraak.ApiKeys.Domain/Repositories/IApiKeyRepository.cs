using Ashraak.ApiKeys.Domain.Aggregates.ApiKey;

namespace Ashraak.ApiKeys.Domain.Repositories;

public interface IApiKeyRepository
{
    void Add(ApiKey apiKey);
    Task<ApiKey?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken);
    Task<ApiKey?> GetByPrefixAsync(string keyPrefix, CancellationToken cancellationToken);
    Task<IReadOnlyList<ApiKey>> ListAsync(Guid tenantId, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken);
}
