using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetSchedule;

public sealed record GetScheduleQuery(
    Guid TenantId,
    Guid UserId,
    Guid ScheduleId) : IRequest<Result<ScheduleDetailDto>>;
