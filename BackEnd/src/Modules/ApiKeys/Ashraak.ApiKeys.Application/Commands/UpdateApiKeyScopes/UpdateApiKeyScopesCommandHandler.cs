using Ashraak.ApiKeys.Application.Mapping;
using Ashraak.ApiKeys.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.ApiKeys.Dtos;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.ApiKeys.Application.Commands.UpdateApiKeyScopes;

internal sealed class UpdateApiKeyScopesCommandHandler(
    IApiKeyRepository repository,
    IAuthPermissionChecker permissionChecker,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateApiKeyScopesCommand, Result<ApiKeyContract>>
{
    public async Task<Result<ApiKeyContract>> Handle(
        UpdateApiKeyScopesCommand request,
        CancellationToken cancellationToken)
    {
        var authError = await ApiKeysAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var apiKey = await repository.GetByIdAsync(request.TenantId, request.ApiKeyId, cancellationToken);
        if (apiKey is null)
            return Error.NotFound("ApiKeys.NotFound", "API key not found.");

        apiKey.UpdateScopes(request.Scopes, request.UserId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiKeyMapper.ToContract(apiKey);
    }
}
