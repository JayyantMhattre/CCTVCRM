using Ashraak.ApiKeys.Application.Abstractions;
using Ashraak.ApiKeys.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.ApiKeys.Interfaces;

namespace Ashraak.ApiKeys.Infrastructure.Services;

internal sealed class ApiKeyValidator(
    IApiKeyRepository repository,
    IApiKeyHasher hasher) : IApiKeyValidator
{
    public async Task<ApiKeyValidationResult?> ValidateAsync(
        string plaintextKey,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(plaintextKey))
            return null;

        var trimmed = plaintextKey.Trim();
        if (!trimmed.StartsWith("ashk_", StringComparison.OrdinalIgnoreCase))
            return null;

        var prefixEnd = trimmed.LastIndexOf('_');
        if (prefixEnd <= 4)
            return null;

        var prefix = trimmed[..prefixEnd];
        var apiKey = await repository.GetByPrefixAsync(prefix, cancellationToken);
        if (apiKey is null || !apiKey.IsActive)
            return null;

        if (!hasher.Verify(trimmed, apiKey.HashedSecret))
            return null;

        return new ApiKeyValidationResult(
            apiKey.Id.Value,
            apiKey.TenantId,
            apiKey.KeyPrefix,
            apiKey.Scopes);
    }
}
