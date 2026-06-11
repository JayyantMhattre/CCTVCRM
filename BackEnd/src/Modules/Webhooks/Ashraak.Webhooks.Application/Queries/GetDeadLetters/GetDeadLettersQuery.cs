using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Queries.GetDeadLetters;

public sealed record GetDeadLettersQuery(
    Guid TenantId,
    Guid UserId,
    Guid? SubscriptionId = null,
    string? EventName = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    int Limit = 100) : IRequest<Result<IReadOnlyList<WebhookDeadLetterContract>>>;
