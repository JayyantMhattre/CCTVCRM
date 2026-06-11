using Ashraak.Files.Domain.Aggregates.FileRecord;
using Ashraak.Files.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Files.Infrastructure.Persistence.Repositories;

internal sealed class FileRecordRepository(FilesDbContext context) : IFileRecordRepository
{
    public Task<FileRecord?> GetByIdAsync(FileRecordId id, CancellationToken cancellationToken = default) =>
        context.FileRecords.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public void Add(FileRecord record) => context.FileRecords.Add(record);

    public void Update(FileRecord record) => context.FileRecords.Update(record);
}
