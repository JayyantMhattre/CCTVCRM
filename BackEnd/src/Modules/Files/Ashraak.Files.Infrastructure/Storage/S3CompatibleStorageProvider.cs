using Microsoft.Extensions.Options;

namespace Ashraak.Files.Infrastructure.Storage;

internal sealed class S3CompatibleStorageProvider(
    IHttpClientFactory httpClientFactory,
    IOptions<StorageOptions> options) : HttpObjectStorageProvider(httpClientFactory)
{
    public override string ProviderName => "S3";

    protected override Uri BuildObjectUri(string storagePath)
    {
        var cfg = options.Value.S3;
        var baseUrl = cfg.ServiceUrl.TrimEnd('/');
        return new Uri($"{baseUrl}/{cfg.Bucket}/{storagePath}");
    }

    protected override Uri BuildHealthUri() => BuildObjectUri(".health");

    protected override void ApplyAuthHeaders(HttpRequestMessage request)
    {
        var cfg = options.Value.S3;
        if (!string.IsNullOrWhiteSpace(cfg.AuthHeaderName) && cfg.AuthHeaderValue is not null)
            request.Headers.TryAddWithoutValidation(cfg.AuthHeaderName, cfg.AuthHeaderValue);
    }
}
