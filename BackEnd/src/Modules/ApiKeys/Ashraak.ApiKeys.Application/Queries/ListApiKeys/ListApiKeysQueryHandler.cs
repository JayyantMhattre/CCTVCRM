using Ashraak.ApiKeys.Application.Mapping;
using Ashraak.ApiKeys.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.ApiKeys.Dtos;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.ApiKeys.Application.Queries.ListApiKeys;

internal sealed class ListApiKeysQueryHandler(
    IApiKeyRepository repository,
    IAuthPermissionChecker permissionChecker) : IRequestHandler<ListApiKeysQuery, Result<IReadOnlyList<ApiKeyContract>>>
{
    public async Task<Result<IReadOnlyList<ApiKeyContract>>> Handle(
        ListApiKeysQuery request,
        CancellationToken cancellationToken)
    {
        var authError = await ApiKeysAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var keys = await repository.ListAsync(request.TenantId, cancellationToken);
        return keys.Select(ApiKeyMapper.ToContract).ToList();
    }
}
