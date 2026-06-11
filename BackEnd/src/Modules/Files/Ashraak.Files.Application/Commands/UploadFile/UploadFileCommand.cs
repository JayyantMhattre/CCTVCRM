using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Files.Application.Commands.UploadFile;

public sealed record UploadFileCommand(
    Guid TenantId,
    Guid UploadedByUserId,
    string FileName,
    string ContentType,
    Stream Content,
    long ContentLength) : IRequest<Result<UploadFileResult>>;

public sealed record UploadFileResult(
    Guid Id,
    string FileName,
    string ContentType,
    long Size,
    DateTime UploadedOnUtc);
