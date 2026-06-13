using Ashraak.Cctv.Integration.Application;
using Ashraak.Files.Application.Commands.UploadFile;
using MediatR;

namespace Ashraak.Cctv.Integration.Infrastructure.Services;

internal sealed class CctvFileStore(IMediator mediator) : ICctvFileStore
{
    public async Task<Guid> StorePdfAsync(
        Guid tenantId,
        Guid userId,
        string fileName,
        byte[] content,
        CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(content);
        var result = await mediator.Send(
            new UploadFileCommand(tenantId, userId, fileName, "application/pdf", stream, content.Length),
            cancellationToken);

        if (result.IsFailure)
            throw new InvalidOperationException(result.Error.Description);

        return result.Value!.Id;
    }
}
