using Ashraak.ApiKeys.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.ApiKeys.Application.Commands.RevokeApiKey;

internal sealed class RevokeApiKeyCommandHandler(
    IApiKeyRepository repository,
    IAuthPermissionChecker permissionChecker,
    IUnitOfWork unitOfWork) : IRequestHandler<RevokeApiKeyCommand, Result>
{
    public async Task<Result> Handle(RevokeApiKeyCommand request, CancellationToken cancellationToken)
    {
        var authError = await ApiKeysAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var apiKey = await repository.GetByIdAsync(request.TenantId, request.ApiKeyId, cancellationToken);
        if (apiKey is null)
            return Error.NotFound("ApiKeys.NotFound", "API key not found.");

        apiKey.Revoke(request.UserId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
