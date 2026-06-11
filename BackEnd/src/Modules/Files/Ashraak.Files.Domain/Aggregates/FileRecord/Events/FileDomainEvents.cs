using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Files.Domain.Aggregates.FileRecord.Events;

public sealed record FileUploadedDomainEvent(
    Guid FileId,
    Guid TenantId,
    Guid UploadedByUserId,
    string FileName,
    string ContentType,
    long Size) : DomainEvent;

public sealed record FileDownloadedDomainEvent(
    Guid FileId,
    Guid TenantId,
    Guid DownloadedByUserId,
    string FileName) : DomainEvent;

public sealed record FileDeletedDomainEvent(
    Guid FileId,
    Guid TenantId,
    Guid DeletedByUserId,
    string FileName) : DomainEvent;
