using Ashraak.SharedKernel.Contracts.ApiKeys.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.ApiKeys.Application.Commands.UpdateApiKeyScopes;

public sealed record UpdateApiKeyScopesCommand(
    Guid TenantId,
    Guid UserId,
    Guid ApiKeyId,
    IReadOnlyList<string> Scopes) : IRequest<Result<ApiKeyContract>>;
