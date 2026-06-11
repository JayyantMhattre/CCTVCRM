using Ashraak.SharedKernel.Contracts.Storage.Dtos;
using Microsoft.Extensions.Options;

namespace Ashraak.Files.Infrastructure.Storage;

internal sealed class LocalDiskStorageProvider(IOptions<StorageOptions> options) : IStorageProvider
{
    public string ProviderName => "Local";

    public async Task<FileStorageResult> PutAsync(
        FileUploadRequest request,
        string storagePath,
        CancellationToken cancellationToken)
    {
        var fullPath = GetFullPath(storagePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var file = File.Create(fullPath);
        await request.Content.CopyToAsync(file, cancellationToken);

        var size = request.ContentLength ?? new FileInfo(fullPath).Length;
        return new FileStorageResult(storagePath, size);
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken)
    {
        var fullPath = GetFullPath(storagePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string storagePath, CancellationToken cancellationToken) =>
        Task.FromResult(File.Exists(GetFullPath(storagePath)));

    public Task<Stream> OpenReadAsync(string storagePath, CancellationToken cancellationToken)
    {
        var fullPath = GetFullPath(storagePath);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Blob not found.", storagePath);

        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult(stream);
    }

    public Task<bool> CheckHealthAsync(CancellationToken cancellationToken)
    {
        var root = GetRoot();
        if (!Directory.Exists(root))
            Directory.CreateDirectory(root);

        var probe = Path.Combine(root, ".health");
        File.WriteAllText(probe, DateTime.UtcNow.ToString("O"));
        File.Delete(probe);
        return Task.FromResult(true);
    }

    private string GetRoot()
    {
        var root = options.Value.Local.RootPath;
        return Path.IsPathRooted(root) ? root : Path.Combine(AppContext.BaseDirectory, root);
    }

    private string GetFullPath(string storagePath) =>
        Path.Combine(GetRoot(), storagePath.Replace('/', Path.DirectorySeparatorChar));
}
