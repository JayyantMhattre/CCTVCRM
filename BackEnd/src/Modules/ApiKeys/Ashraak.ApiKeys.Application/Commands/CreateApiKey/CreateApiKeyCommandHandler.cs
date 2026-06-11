using Ashraak.ApiKeys.Application.Abstractions;
using Ashraak.ApiKeys.Domain.Aggregates.ApiKey;
using Ashraak.ApiKeys.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.ApiKeys.Dtos;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Options;

namespace Ashraak.ApiKeys.Application.Commands.CreateApiKey;

internal sealed class CreateApiKeyCommandHandler(
    IApiKeyRepository repository,
    IAuthPermissionChecker permissionChecker,
    IApiKeyGenerator keyGenerator,
    IApiKeyHasher keyHasher,
    IOptions<ApiKeysOptions> options,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateApiKeyCommand, Result<ApiKeyCreatedContract>>
{
    public async Task<Result<ApiKeyCreatedContract>> Handle(
        CreateApiKeyCommand request,
        CancellationToken cancellationToken)
    {
        var authError = await ApiKeysAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        if (string.IsNullOrWhiteSpace(request.Name))
            return Error.Validation("ApiKeys.NameRequired", "API key name is required.");

        if (await repository.ExistsByNameAsync(request.TenantId, request.Name.Trim(), cancellationToken))
            return Error.Conflict("ApiKeys.NameExists", "An API key with this name already exists.");

        var env = options.Value.Environment;
        var (plaintextKey, keyPrefix) = keyGenerator.Generate(env);
        var expiresOnUtc = request.ExpiresOnUtc;
        if (expiresOnUtc is null && options.Value.DefaultExpiryDays > 0)
            expiresOnUtc = DateTime.UtcNow.AddDays(options.Value.DefaultExpiryDays);

        var apiKey = ApiKey.Create(
            ApiKeyId.New(),
            request.TenantId,
            request.Name,
            request.Description ?? string.Empty,
            keyPrefix,
            keyHasher.Hash(plaintextKey),
            env,
            request.Scopes,
            request.UserId,
            expiresOnUtc);

        repository.Add(apiKey);
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
