using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetEngineerTodaySchedules;

public sealed record GetEngineerTodaySchedulesQuery(
    Guid TenantId,
    Guid UserId) : IRequest<Result<IReadOnlyList<ScheduleSummaryDto>>>;
