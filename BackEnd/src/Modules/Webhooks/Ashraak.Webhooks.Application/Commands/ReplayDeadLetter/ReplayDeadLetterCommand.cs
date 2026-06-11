using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Commands.ReplayDeadLetter;

public sealed record ReplayDeadLetterCommand(
    Guid DeadLetterId,
    Guid TenantId,
    Guid UserId) : IRequest<Result<WebhookDeliveryContract>>;
