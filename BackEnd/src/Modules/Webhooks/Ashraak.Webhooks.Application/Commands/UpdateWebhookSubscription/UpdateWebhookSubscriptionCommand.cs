using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Commands.UpdateWebhookSubscription;

public sealed record UpdateWebhookSubscriptionCommand(
    Guid SubscriptionId,
    Guid TenantId,
    Guid UserId,
    string Name,
    string EndpointUrl,
    bool Enabled,
    IReadOnlyList<string>? SubscribedEventNames = null) : IRequest<Result<WebhookSubscriptionContract>>;
