using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.ApiKeys.Application.Commands.RevokeApiKey;

public sealed record RevokeApiKeyCommand(
    Guid TenantId,
    Guid UserId,
    Guid ApiKeyId) : IRequest<Result>;
