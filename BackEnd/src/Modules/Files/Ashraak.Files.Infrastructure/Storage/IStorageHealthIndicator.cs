namespace Ashraak.Files.Infrastructure.Storage;

/// <summary>Public health probe for host readiness checks.</summary>
public interface IStorageHealthIndicator
{
    Task<bool> CheckHealthAsync(CancellationToken cancellationToken = default);
}

internal sealed class StorageHealthIndicator(IStorageProvider provider) : IStorageHealthIndicator
{
    public Task<bool> CheckHealthAsync(CancellationToken cancellationToken = default) =>
        provider.CheckHealthAsync(cancellationToken);
}
