using Ashraak.SharedKernel.Contracts.Storage.Dtos;

namespace Ashraak.Files.Infrastructure.Storage;

internal interface IStorageProvider
{
    string ProviderName { get; }

    Task<FileStorageResult> PutAsync(FileUploadRequest request, string storagePath, CancellationToken cancellationToken);

    Task DeleteAsync(string storagePath, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string storagePath, CancellationToken cancellationToken);

    Task<Stream> OpenReadAsync(string storagePath, CancellationToken cancellationToken);

    Task<bool> CheckHealthAsync(CancellationToken cancellationToken);
}
