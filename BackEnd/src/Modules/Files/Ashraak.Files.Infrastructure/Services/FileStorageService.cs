using Ashraak.SharedKernel.Contracts.Storage.Dtos;
using Ashraak.SharedKernel.Contracts.Storage.Interfaces;
using Ashraak.Files.Infrastructure.Storage;

namespace Ashraak.Files.Infrastructure.Services;

internal sealed class FileStorageService(IStorageProvider provider) : IFileStorage
{
    public async Task<FileStorageResult> UploadAsync(FileUploadRequest request, CancellationToken cancellationToken = default)
    {
        var path = StoragePathBuilder.Build(request.TenantId, request.FileId, request.FileName);
        return await provider.PutAsync(request, path, cancellationToken);
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default) =>
        provider.DeleteAsync(storagePath, cancellationToken);

    public Task<string> GetUrlAsync(string storagePath, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var fileId = StoragePathBuilder.TryExtractFileId(storagePath);
        if (fileId is null)
            return Task.FromResult($"/api/v1/files/by-path?path={Uri.EscapeDataString(storagePath)}");

        return Task.FromResult($"/api/v1/files/{fileId:D}");
    }

    public Task<bool> ExistsAsync(string storagePath, CancellationToken cancellationToken = default) =>
        provider.ExistsAsync(storagePath, cancellationToken);

    public Task<Stream> OpenReadAsync(string storagePath, CancellationToken cancellationToken = default) =>
        provider.OpenReadAsync(storagePath, cancellationToken);
}
