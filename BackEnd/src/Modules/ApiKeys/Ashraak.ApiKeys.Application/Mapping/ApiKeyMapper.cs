using Ashraak.ApiKeys.Domain.Aggregates.ApiKey;
using Ashraak.SharedKernel.Contracts.ApiKeys.Dtos;

namespace Ashraak.ApiKeys.Application.Mapping;

internal static class ApiKeyMapper
{
    public static ApiKeyContract ToContract(ApiKey apiKey) =>
        new(
            apiKey.Id.Value,
            apiKey.TenantId,
            apiKey.Name,
            apiKey.Description,
            apiKey.KeyPrefix,
            apiKey.Environment,
            apiKey.Scopes,
            apiKey.CreatedBy,
            apiKey.CreatedOnUtc,
            apiKey.ExpiresOnUtc,
            apiKey.LastUsedOnUtc,
            apiKey.RevokedOnUtc,
            apiKey.Enabled && apiKey.IsActive,
            apiKey.RequestCount,
            apiKey.SuccessCount,
            apiKey.FailureCount,
            apiKey.LastCorrelationId);
}
