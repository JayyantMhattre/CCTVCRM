using Ashraak.SharedKernel.Contracts.Webhooks.Events;
using Ashraak.Webhooks.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ashraak.Webhooks.Application.EventHandlers;

internal sealed class WebhookRequestedEventHandler(
    IWebhookDispatcher dispatcher,
    ILogger<WebhookRequestedEventHandler> logger) : INotificationHandler<WebhookRequestedEvent>
{
    public async Task Handle(WebhookRequestedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Dispatching webhook event {EventName} for tenant {TenantId} (correlation {CorrelationId})",
            notification.EventName,
            notification.TenantId,
            notification.CorrelationId);

        await dispatcher.DispatchAsync(notification, cancellationToken);
    }
}
