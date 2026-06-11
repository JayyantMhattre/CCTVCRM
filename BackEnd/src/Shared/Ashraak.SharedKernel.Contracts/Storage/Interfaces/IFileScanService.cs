using Ashraak.SharedKernel.Contracts.Storage.Dtos;

namespace Ashraak.SharedKernel.Contracts.Storage.Interfaces;

/// <summary>
/// Extension point for antivirus scanning. Default implementation is a no-op stub.
/// </summary>
public interface IFileScanService
{
    Task<FileScanResult> ScanAsync(Stream content, string fileName, CancellationToken cancellationToken = default);
}
