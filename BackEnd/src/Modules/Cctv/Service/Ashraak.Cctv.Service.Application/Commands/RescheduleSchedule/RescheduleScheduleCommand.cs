using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.RescheduleSchedule;

public sealed record RescheduleScheduleCommand(
    Guid TenantId,
    Guid UserId,
    Guid ScheduleId,
    DateOnly NewScheduledDate,
    string Reason,
    uint RowVersion) : IRequest<Result<ScheduleDetailDto>>;
