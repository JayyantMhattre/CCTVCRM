using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Queries.GetSubscriptions;

public sealed record GetSubscriptionsQuery(Guid TenantId, Guid UserId)
    : IRequest<Result<IReadOnlyList<WebhookSubscriptionContract>>>;
