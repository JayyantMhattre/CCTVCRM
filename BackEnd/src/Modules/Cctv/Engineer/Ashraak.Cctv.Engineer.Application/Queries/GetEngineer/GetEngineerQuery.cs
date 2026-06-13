using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Engineer.Application.Queries.GetEngineer;

public sealed record GetEngineerQuery(
    Guid TenantId,
    Guid UserId,
    Guid EngineerId) : IRequest<Result<EngineerDetailDto>>;
