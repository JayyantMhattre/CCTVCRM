using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Queries.GetDeliveryHistory;

public sealed record GetDeliveryHistoryQuery(
    Guid TenantId,
    Guid UserId,
    Guid? SubscriptionId,
    string? EventName,
    string? Status,
    DateTime? FromUtc,
    DateTime? ToUtc,
    int Limit = 100) : IRequest<Result<IReadOnlyList<WebhookDeliveryContract>>>;
