using Ashraak.Files.Domain.Aggregates.FileRecord;
using Ashraak.Files.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Storage.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Files.Application.Queries.DownloadFile;

internal sealed class DownloadFileQueryHandler(
    IFileRecordRepository repository,
    IFileStorage fileStorage,
    IUnitOfWork unitOfWork) : IRequestHandler<DownloadFileQuery, Result<DownloadFileResult>>
{
    public async Task<Result<DownloadFileResult>> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
    {
        var record = await repository.GetByIdAsync(FileRecordId.From(request.FileId), cancellationToken);
        if (record is null || record.TenantId != request.TenantId || record.IsDeleted)
            return Error.NotFound("Files.NotFound", "File not found.");

        record.RecordDownload(request.RequestedByUserId);
        repository.Update(record);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var stream = await fileStorage.OpenReadAsync(record.StoragePath, cancellationToken);
        return new DownloadFileResult(stream, record.FileName, record.ContentType);
    }
}
