using Ashraak.Files.Domain.Aggregates.FileRecord.Events;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Files.Domain.Aggregates.FileRecord;

/// <summary>Tenant-scoped file metadata. Blob bytes live in the configured storage provider.</summary>
public sealed class FileRecord : AggregateRoot<FileRecordId>
{
    private FileRecord(FileRecordId id) : base(id) { }

    public Guid TenantId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long Size { get; private set; }
    public string StoragePath { get; private set; } = string.Empty;
    public Guid UploadedBy { get; private set; }
    public DateTime UploadedOnUtc { get; private set; }
    public DateTime? DeletedOnUtc { get; private set; }

    public bool IsDeleted => DeletedOnUtc.HasValue;

    public static FileRecord Create(
        FileRecordId id,
        Guid tenantId,
        string fileName,
        string contentType,
        long size,
        string storagePath,
        Guid uploadedBy)
    {
        var record = new FileRecord(id)
        {
            TenantId = tenantId,
            FileName = fileName,
            ContentType = contentType,
            Size = size,
            StoragePath = storagePath,
            UploadedBy = uploadedBy,
            UploadedOnUtc = DateTime.UtcNow
        };

        record.RaiseDomainEvent(new FileUploadedDomainEvent(
            record.Id.Value,
            tenantId,
            uploadedBy,
            fileName,
            contentType,
            size));

        return record;
    }

    public void RecordDownload(Guid downloadedBy)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot download a deleted file.");

        RaiseDomainEvent(new FileDownloadedDomainEvent(
            Id.Value,
            TenantId,
            downloadedBy,
            FileName));
    }

    public void SoftDelete(Guid deletedBy)
    {
        if (IsDeleted)
            return;

        DeletedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new FileDeletedDomainEvent(
            Id.Value,
            TenantId,
            deletedBy,
            FileName));
    }
}
