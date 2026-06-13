using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.AssignEngineer;

public sealed record AssignEngineerCommand(
    Guid TenantId,
    Guid UserId,
    Guid ScheduleId,
    Guid EngineerId,
    uint RowVersion) : IRequest<Result<ScheduleDetailDto>>;
