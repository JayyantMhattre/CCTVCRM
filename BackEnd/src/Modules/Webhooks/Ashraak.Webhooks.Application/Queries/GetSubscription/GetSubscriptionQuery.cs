using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Queries.GetSubscription;

public sealed record GetSubscriptionQuery(Guid SubscriptionId, Guid TenantId, Guid UserId)
    : IRequest<Result<WebhookSubscriptionContract>>;
