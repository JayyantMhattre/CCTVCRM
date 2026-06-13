using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Engineer.Application.Queries.GetEngineerWorkload;

public sealed record GetEngineerWorkloadQuery(
    Guid TenantId,
    Guid UserId,
    Guid EngineerId) : IRequest<Result<EngineerWorkloadDto>>;
