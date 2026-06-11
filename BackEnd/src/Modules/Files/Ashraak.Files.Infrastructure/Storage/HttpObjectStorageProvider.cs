using Ashraak.SharedKernel.Contracts.Storage.Dtos;
using System.Net.Http.Headers;

namespace Ashraak.Files.Infrastructure.Storage;

/// <summary>HTTP PUT/GET/DELETE adapter for S3-compatible and Azure Blob gateways.</summary>
internal abstract class HttpObjectStorageProvider(IHttpClientFactory httpClientFactory) : IStorageProvider
{
    public abstract string ProviderName { get; }

    protected abstract Uri BuildObjectUri(string storagePath);

    protected virtual void ApplyAuthHeaders(HttpRequestMessage request)
    {
    }

    public async Task<FileStorageResult> PutAsync(
        FileUploadRequest request,
        string storagePath,
        CancellationToken cancellationToken)
    {
        using var http = httpClientFactory.CreateClient(ProviderName);
        using var message = new HttpRequestMessage(HttpMethod.Put, BuildObjectUri(storagePath))
        {
            Content = new StreamContent(request.Content)
        };
        message.Content.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);
        ApplyAuthHeaders(message);

        var response = await http.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();

        var size = request.ContentLength ?? request.Content.Length;
        return new FileStorageResult(storagePath, size);
    }

    public async Task DeleteAsync(string storagePath, CancellationToken cancellationToken)
    {
        using var http = httpClientFactory.CreateClient(ProviderName);
        using var message = new HttpRequestMessage(HttpMethod.Delete, BuildObjectUri(storagePath));
        ApplyAuthHeaders(message);

        var response = await http.SendAsync(message, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return;

        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> ExistsAsync(string storagePath, CancellationToken cancellationToken)
    {
        using var http = httpClientFactory.CreateClient(ProviderName);
        using var message = new HttpRequestMessage(HttpMethod.Head, BuildObjectUri(storagePath));
        ApplyAuthHeaders(message);

        var response = await http.SendAsync(message, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<Stream> OpenReadAsync(string storagePath, CancellationToken cancellationToken)
    {
        using var http = httpClientFactory.CreateClient(ProviderName);
        using var message = new HttpRequestMessage(HttpMethod.Get, BuildObjectUri(storagePath));
        ApplyAuthHeaders(message);

        var response = await http.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();
        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        return new MemoryStream(bytes);
    }

    public async Task<bool> CheckHealthAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var http = httpClientFactory.CreateClient(ProviderName);
            using var message = new HttpRequestMessage(HttpMethod.Head, BuildHealthUri());
            ApplyAuthHeaders(message);
            var response = await http.SendAsync(message, cancellationToken);
            return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound;
        }
        catch
        {
            return false;
        }
    }

    protected abstract Uri BuildHealthUri();
}
