using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Queries.GetDeadLetter;

public sealed record GetDeadLetterQuery(
    Guid DeadLetterId,
    Guid TenantId,
    Guid UserId) : IRequest<Result<WebhookDeadLetterContract>>;
