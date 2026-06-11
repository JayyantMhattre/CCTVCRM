using Ashraak.SharedKernel.Contracts.ApiKeys.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.ApiKeys.Application.Queries.GetApiKey;

public sealed record GetApiKeyQuery(Guid TenantId, Guid UserId, Guid ApiKeyId) : IRequest<Result<ApiKeyContract>>;
