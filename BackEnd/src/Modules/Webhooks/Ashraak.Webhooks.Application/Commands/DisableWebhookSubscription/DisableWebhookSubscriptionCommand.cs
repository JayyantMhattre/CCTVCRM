using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Commands.DisableWebhookSubscription;

public sealed record DisableWebhookSubscriptionCommand(
    Guid SubscriptionId,
    Guid TenantId,
    Guid UserId) : IRequest<Result<WebhookSubscriptionContract>>;
