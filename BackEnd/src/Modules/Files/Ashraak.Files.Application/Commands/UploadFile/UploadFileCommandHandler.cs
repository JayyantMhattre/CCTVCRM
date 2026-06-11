using Ashraak.Files.Domain.Aggregates.FileRecord;
using Ashraak.Files.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Storage.Dtos;
using Ashraak.SharedKernel.Contracts.Storage.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Options;

namespace Ashraak.Files.Application.Commands.UploadFile;

internal sealed class UploadFileCommandHandler(
    IFileRecordRepository repository,
    IFileStorage fileStorage,
    IFileScanService fileScanService,
    IOptions<StorageValidationOptions> validationOptions,
    IUnitOfWork unitOfWork) : IRequestHandler<UploadFileCommand, Result<UploadFileResult>>
{
    public async Task<Result<UploadFileResult>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var options = validationOptions.Value;

        if (request.ContentLength <= 0)
            return Error.Validation("Files.Empty", "File is empty.");

        if (options.MaxUploadBytes > 0 && request.ContentLength > options.MaxUploadBytes)
            return Error.Validation("Files.TooLarge", $"File exceeds maximum size of {options.MaxUploadBytes} bytes.");

        if (options.AllowedContentTypes.Count > 0
            && !options.AllowedContentTypes.Contains(request.ContentType, StringComparer.OrdinalIgnoreCase))
            return Error.Validation("Files.ContentType", "Content type is not allowed.");

        var scan = await fileScanService.ScanAsync(request.Content, request.FileName, cancellationToken);
        if (!scan.IsClean)
            return Error.Validation("Files.Infected", scan.ThreatName ?? "File failed virus scan.");

        if (request.Content.CanSeek)
            request.Content.Position = 0;

        var fileId = FileRecordId.New();

        var stored = await fileStorage.UploadAsync(
            new FileUploadRequest(
                request.TenantId,
                request.UploadedByUserId,
                fileId.Value,
                request.FileName,
                request.ContentType,
                request.Content,
                request.ContentLength),
            cancellationToken);

        var record = FileRecord.Create(
            fileId,
            request.TenantId,
            request.FileName,
            request.ContentType,
            stored.SizeBytes,
            stored.StoragePath,
            request.UploadedByUserId);

        repository.Add(record);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UploadFileResult(
            record.Id.Value,
            record.FileName,
            record.ContentType,
            record.Size,
            record.UploadedOnUtc);
    }

}

/// <summary>Upload validation rules bound from Storage configuration.</summary>
public sealed class StorageValidationOptions
{
    public long MaxUploadBytes { get; set; } = 10 * 1024 * 1024;
    public List<string> AllowedContentTypes { get; set; } = [];
}
