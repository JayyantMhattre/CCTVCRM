using Ashraak.Webhooks.Application.Abstractions;
using System.Threading.Channels;

namespace Ashraak.Webhooks.Infrastructure.Delivery;

internal sealed class WebhookDeliveryQueue : IWebhookDeliveryQueue
{
    private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>(
        new UnboundedChannelOptions { SingleReader = false, SingleWriter = false });

    public ValueTask EnqueueAsync(Guid deliveryId, CancellationToken cancellationToken = default) =>
        _channel.Writer.WriteAsync(deliveryId, cancellationToken);

    public IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken cancellationToken) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
