using Ashraak.SharedKernel.Contracts.ApiKeys.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.ApiKeys.Application.Commands.RotateApiKey;

public sealed record RotateApiKeyCommand(
    Guid TenantId,
    Guid UserId,
    Guid ApiKeyId) : IRequest<Result<ApiKeyCreatedContract>>;
