using Ashraak.SharedKernel.Contracts.ApiKeys.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.ApiKeys.Application.Commands.CreateApiKey;

public sealed record CreateApiKeyCommand(
    Guid TenantId,
    Guid UserId,
    string Name,
    string Description,
    IReadOnlyList<string> Scopes,
    DateTime? ExpiresOnUtc) : IRequest<Result<ApiKeyCreatedContract>>;
