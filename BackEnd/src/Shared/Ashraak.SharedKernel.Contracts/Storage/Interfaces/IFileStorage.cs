using Ashraak.SharedKernel.Contracts.Storage.Dtos;

namespace Ashraak.SharedKernel.Contracts.Storage.Interfaces;

/// <summary>
/// Platform file storage contract. Implemented by the Files module.
/// Other modules depend on this interface only — not on Files.Infrastructure.
/// </summary>
public interface IFileStorage
{
    Task<FileStorageResult> UploadAsync(FileUploadRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns an authenticated access path or URI. Never returns anonymous public URLs.
    /// </summary>
    Task<string> GetUrlAsync(string storagePath, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string storagePath, CancellationToken cancellationToken = default);

    Task<Stream> OpenReadAsync(string storagePath, CancellationToken cancellationToken = default);
}
