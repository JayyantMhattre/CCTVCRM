using Ashraak.ApiKeys.Application.Abstractions;
using Ashraak.ApiKeys.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.ApiKeys.Dtos;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Options;

namespace Ashraak.ApiKeys.Application.Commands.RotateApiKey;

internal sealed class RotateApiKeyCommandHandler(
    IApiKeyRepository repository,
    IAuthPermissionChecker permissionChecker,
    IApiKeyGenerator keyGenerator,
    IApiKeyHasher keyHasher,
    IOptions<ApiKeysOptions> options,
    IUnitOfWork unitOfWork) : IRequestHandler<RotateApiKeyCommand, Result<ApiKeyCreatedContract>>
{
    public async Task<Result<ApiKeyCreatedContract>> Handle(
        RotateApiKeyCommand request,
        CancellationToken cancellationToken)
    {
        var authError = await ApiKeysAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var apiKey = await repository.GetByIdAsync(request.TenantId, request.ApiKeyId, cancellationToken);
        if (apiKey is null)
            return Error.NotFound("ApiKeys.NotFound", "API key not found.");

        var (plaintextKey, keyPrefix) = keyGenerator.Generate(options.Value.Environment);
        apiKey.Rotate(keyPrefix, keyHasher.Hash(plaintextKey), request.UserId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ApiKeyCreatedContract(
            apiKey.Id.Value,
            apiKey.TenantId,
            apiKey.Name,
            apiKey.KeyPrefix,
            plaintextKey,
            apiKey.Scopes,
            apiKey.CreatedOnUtc,
            apiKey.ExpiresOnUtc);
    }
}
