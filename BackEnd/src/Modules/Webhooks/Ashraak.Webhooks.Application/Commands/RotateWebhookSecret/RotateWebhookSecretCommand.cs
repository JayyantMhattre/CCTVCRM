using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Commands.RotateWebhookSecret;

public sealed record RotateWebhookSecretCommand(
    Guid SubscriptionId,
    Guid TenantId,
    Guid UserId) : IRequest<Result<WebhookSubscriptionSecretContract>>;
