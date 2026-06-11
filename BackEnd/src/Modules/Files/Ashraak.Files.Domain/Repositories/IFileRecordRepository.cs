using Ashraak.Files.Domain.Aggregates.FileRecord;

namespace Ashraak.Files.Domain.Repositories;

public interface IFileRecordRepository
{
    Task<FileRecord?> GetByIdAsync(FileRecordId id, CancellationToken cancellationToken = default);
    void Add(FileRecord record);
    void Update(FileRecord record);
}
