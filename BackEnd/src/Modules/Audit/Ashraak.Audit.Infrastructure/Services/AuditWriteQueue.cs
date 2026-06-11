using System.Threading.Channels;
using Ashraak.SharedKernel.Contracts.Audit.Dtos;

namespace Ashraak.Audit.Infrastructure.Services;

/// <summary>
/// Unbounded channel-backed queue for audit writes.
/// Chosen to guarantee producer progress without blocking request paths.
/// </summary>
internal sealed class AuditWriteQueue : IAuditWriteQueue
{
    private readonly Channel<AuditEntryDto> _channel = Channel.CreateUnbounded<AuditEntryDto>(
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });

    /// <inheritdoc />
    public ValueTask EnqueueAsync(AuditEntryDto entry, CancellationToken cancellationToken = default)
    {
        // TryWrite avoids any potential async state machine overhead in the fast path.
        if (_channel.Writer.TryWrite(entry))
            return ValueTask.CompletedTask;

        return _channel.Writer.WriteAsync(entry, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<AuditEntryDto> DequeueAllAsync(CancellationToken cancellationToken) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
