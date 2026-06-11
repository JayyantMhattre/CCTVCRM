using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Files.Application.Commands.DeleteFile;

public sealed record DeleteFileCommand(Guid FileId, Guid TenantId, Guid DeletedByUserId) : IRequest<Result>;
