namespace Ashraak.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Polling configuration for <see cref="OutboxProcessorHostedService{TDbContext}"/>.
/// </summary>
public sealed class OutboxProcessorOptions
{
    /// <summary>Delay between polling cycles.</summary>
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>Maximum messages processed per cycle.</summary>
    public int BatchSize { get; set; } = 20;
}
