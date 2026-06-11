using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Files.Application.Queries.GetFileUrl;

public sealed record GetFileUrlQuery(Guid FileId, Guid TenantId) : IRequest<Result<string>>;
