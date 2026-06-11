using Ashraak.SharedKernel.Contracts.Storage.Dtos;
using Ashraak.SharedKernel.Contracts.Storage.Interfaces;

namespace Ashraak.Files.Infrastructure.Services;

/// <summary>No-op virus scan stub. Replace for production AV integration.</summary>
internal sealed class StubFileScanService : IFileScanService
{
    public Task<FileScanResult> ScanAsync(Stream content, string fileName, CancellationToken cancellationToken = default) =>
        Task.FromResult(new FileScanResult(IsClean: true));
}
