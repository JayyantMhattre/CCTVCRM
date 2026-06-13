using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetEngineerSchedules;

public sealed record GetEngineerSchedulesQuery(
    Guid TenantId,
    Guid UserId,
    DateOnly? FromDate,
    DateOnly? ToDate) : IRequest<Result<IReadOnlyList<ScheduleSummaryDto>>>;
