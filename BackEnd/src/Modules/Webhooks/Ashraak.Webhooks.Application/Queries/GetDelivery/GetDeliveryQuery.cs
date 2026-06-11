using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Queries.GetDelivery;

public sealed record GetDeliveryQuery(Guid DeliveryId, Guid TenantId, Guid UserId)
    : IRequest<Result<WebhookDeliveryContract>>;
