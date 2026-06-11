namespace Ashraak.SharedKernel.Contracts.Storage.Dtos;

/// <summary>Input for uploading a tenant-scoped blob.</summary>
public sealed record FileUploadRequest(
    Guid TenantId,
    Guid UploadedByUserId,
    Guid FileId,
    string FileName,
    string ContentType,
    Stream Content,
    long? ContentLength = null);
