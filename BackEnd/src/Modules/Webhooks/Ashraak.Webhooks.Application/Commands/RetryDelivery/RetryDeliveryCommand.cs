using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Commands.RetryDelivery;

public sealed record RetryDeliveryCommand(
    Guid DeliveryId,
    Guid TenantId,
    Guid UserId) : IRequest<Result<WebhookDeliveryContract>>;
