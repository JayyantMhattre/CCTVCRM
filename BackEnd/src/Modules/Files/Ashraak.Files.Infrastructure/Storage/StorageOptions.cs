namespace Ashraak.Files.Infrastructure.Storage;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string Provider { get; set; } = "Local";
    public long MaxUploadBytes { get; set; } = 10 * 1024 * 1024;
    public List<string> AllowedContentTypes { get; set; } = ["image/png", "image/jpeg", "application/pdf"];

    public LocalStorageOptions Local { get; set; } = new();
    public S3StorageOptions S3 { get; set; } = new();
    public AzureStorageOptions Azure { get; set; } = new();
}

public sealed class LocalStorageOptions
{
    public string RootPath { get; set; } = "./data/files";
}

public sealed class S3StorageOptions
{
    public string ServiceUrl { get; set; } = string.Empty;
    public string Bucket { get; set; } = "ashraak";
    public string? AuthHeaderName { get; set; }
    public string? AuthHeaderValue { get; set; }
}

public sealed class AzureStorageOptions
{
    public string BlobEndpoint { get; set; } = string.Empty;
    public string Container { get; set; } = "ashraak";
    public string? AuthHeaderName { get; set; }
    public string? AuthHeaderValue { get; set; }
}
