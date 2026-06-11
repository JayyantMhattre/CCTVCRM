using Ashraak.Webhooks.Domain.Aggregates.WebhookDeadLetter;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;

namespace Ashraak.Webhooks.Application.Abstractions;

public interface IDeadLetterService
{
    Task MoveToDeadLetterAsync(
        WebhookDelivery delivery,
        int? failureCode,
        string? failureReason,
        CancellationToken cancellationToken = default);

    Task<WebhookDelivery> ReplayAsync(
        WebhookDeadLetterId deadLetterId,
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
