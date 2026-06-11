using Microsoft.Extensions.Options;

namespace Ashraak.Files.Infrastructure.Storage;

internal sealed class AzureBlobStorageProvider(
    IHttpClientFactory httpClientFactory,
    IOptions<StorageOptions> options) : HttpObjectStorageProvider(httpClientFactory)
{
    public override string ProviderName => "Azure";

    protected override Uri BuildObjectUri(string storagePath)
    {
        var cfg = options.Value.Azure;
        var baseUrl = cfg.BlobEndpoint.TrimEnd('/');
        return new Uri($"{baseUrl}/{cfg.Container}/{storagePath}");
    }

    protected override Uri BuildHealthUri() => BuildObjectUri(".health");

    protected override void ApplyAuthHeaders(HttpRequestMessage request)
    {
        var cfg = options.Value.Azure;
        if (!string.IsNullOrWhiteSpace(cfg.AuthHeaderName) && cfg.AuthHeaderValue is not null)
            request.Headers.TryAddWithoutValidation(cfg.AuthHeaderName, cfg.AuthHeaderValue);
    }
}
