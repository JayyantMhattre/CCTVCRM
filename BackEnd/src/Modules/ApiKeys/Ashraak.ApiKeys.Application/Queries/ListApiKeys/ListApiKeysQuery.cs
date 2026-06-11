using Ashraak.SharedKernel.Contracts.ApiKeys.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.ApiKeys.Application.Queries.ListApiKeys;

public sealed record ListApiKeysQuery(Guid TenantId, Guid UserId) : IRequest<Result<IReadOnlyList<ApiKeyContract>>>;
