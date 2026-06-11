using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Commands.CreateWebhookSubscription;

public sealed record CreateWebhookSubscriptionCommand(
    Guid TenantId,
    Guid UserId,
    string Name,
    string EndpointUrl,
    IReadOnlyList<string>? SubscribedEventNames = null) : IRequest<Result<WebhookSubscriptionSecretContract>>;
