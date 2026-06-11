namespace Ashraak.Files.Infrastructure.Storage;

internal static class StoragePathBuilder
{
    public static string Build(Guid tenantId, Guid fileId, string fileName)
    {
        var safe = SanitizeFileName(fileName);
        return $"{tenantId:D}/files/{fileId:D}/{safe}";
    }

    public static Guid? TryExtractFileId(string storagePath)
    {
        var parts = storagePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3 || !parts[1].Equals("files", StringComparison.OrdinalIgnoreCase))
            return null;

        return Guid.TryParse(parts[2], out var id) ? id : null;
    }

    private static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName);
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return string.IsNullOrWhiteSpace(name) ? "file" : name;
    }
}
