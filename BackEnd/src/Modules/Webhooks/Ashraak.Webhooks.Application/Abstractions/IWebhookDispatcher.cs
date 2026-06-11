using Ashraak.SharedKernel.Contracts.Webhooks.Events;

namespace Ashraak.Webhooks.Application.Abstractions;

public interface IWebhookDispatcher
{
    Task DispatchAsync(WebhookRequestedEvent requestedEvent, CancellationToken cancellationToken = default);
}
