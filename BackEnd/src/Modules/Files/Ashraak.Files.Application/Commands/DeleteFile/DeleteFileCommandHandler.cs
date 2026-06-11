using Ashraak.Files.Domain.Aggregates.FileRecord;
using Ashraak.Files.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Storage.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Files.Application.Commands.DeleteFile;

internal sealed class DeleteFileCommandHandler(
    IFileRecordRepository repository,
    IFileStorage fileStorage,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteFileCommand, Result>
{
    public async Task<Result> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        var record = await repository.GetByIdAsync(FileRecordId.From(request.FileId), cancellationToken);
        if (record is null || record.TenantId != request.TenantId)
            return Error.NotFound("Files.NotFound", "File not found.");

        if (record.IsDeleted)
            return Result.Success();

        record.SoftDelete(request.DeletedByUserId);
        await fileStorage.DeleteAsync(record.StoragePath, cancellationToken);
        repository.Update(record);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
