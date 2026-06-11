using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Files.Application.Queries.DownloadFile;

public sealed record DownloadFileQuery(Guid FileId, Guid TenantId, Guid RequestedByUserId)
    : IRequest<Result<DownloadFileResult>>;

public sealed record DownloadFileResult(
    Stream Content,
    string FileName,
    string ContentType);
