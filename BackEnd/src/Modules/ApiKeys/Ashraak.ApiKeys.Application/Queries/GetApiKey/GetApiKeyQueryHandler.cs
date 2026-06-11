using Ashraak.ApiKeys.Application.Mapping;
using Ashraak.ApiKeys.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.ApiKeys.Dtos;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.ApiKeys.Application.Queries.GetApiKey;

internal sealed class GetApiKeyQueryHandler(
    IApiKeyRepository repository,
    IAuthPermissionChecker permissionChecker) : IRequestHandler<GetApiKeyQuery, Result<ApiKeyContract>>
{
    public async Task<Result<ApiKeyContract>> Handle(
        GetApiKeyQuery request,
        CancellationToken cancellationToken)
    {
        var authError = await ApiKeysAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var apiKey = await repository.GetByIdAsync(request.TenantId, request.ApiKeyId, cancellationToken);
        if (apiKey is null)
            return Error.NotFound("ApiKeys.NotFound", "API key not found.");

        return ApiKeyMapper.ToContract(apiKey);
    }
}
