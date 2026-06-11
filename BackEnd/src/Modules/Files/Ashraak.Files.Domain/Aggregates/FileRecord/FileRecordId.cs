namespace Ashraak.Files.Domain.Aggregates.FileRecord;

public sealed record FileRecordId(Guid Value)
{
    public static FileRecordId New() => new(Guid.NewGuid());
    public static FileRecordId From(Guid value) => new(value);
}
