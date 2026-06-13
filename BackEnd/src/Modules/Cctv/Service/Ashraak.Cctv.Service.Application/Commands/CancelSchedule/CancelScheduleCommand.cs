using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.CancelSchedule;

public sealed record CancelScheduleCommand(
    Guid TenantId,
    Guid UserId,
    Guid ScheduleId,
    string Reason,
    uint RowVersion) : IRequest<Result<ScheduleDetailDto>>;
