namespace Ashraak.Cctv.Integration.Application;

/// <summary>Stores generated PDF bytes via the platform Files module.</summary>
public interface ICctvFileStore
{
    Task<Guid> StorePdfAsync(
        Guid tenantId,
        Guid userId,
        string fileName,
        byte[] content,
        CancellationToken cancellationToken = default);
}
