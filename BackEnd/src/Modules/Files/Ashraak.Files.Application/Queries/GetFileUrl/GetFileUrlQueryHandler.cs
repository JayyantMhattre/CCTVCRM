using Ashraak.Files.Domain.Aggregates.FileRecord;
using Ashraak.Files.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Storage.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Files.Application.Queries.GetFileUrl;

internal sealed class GetFileUrlQueryHandler(
    IFileRecordRepository repository,
    IFileStorage fileStorage) : IRequestHandler<GetFileUrlQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GetFileUrlQuery request, CancellationToken cancellationToken)
    {
        var record = await repository.GetByIdAsync(FileRecordId.From(request.FileId), cancellationToken);
        if (record is null || record.TenantId != request.TenantId || record.IsDeleted)
            return Error.NotFound("Files.NotFound", "File not found.");

        var url = await fileStorage.GetUrlAsync(record.StoragePath, cancellationToken: cancellationToken);
        return url;
    }
}
