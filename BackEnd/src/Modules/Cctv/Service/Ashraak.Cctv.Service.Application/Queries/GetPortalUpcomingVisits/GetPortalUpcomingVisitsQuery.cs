using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetPortalUpcomingVisits;

public sealed record GetPortalUpcomingVisitsQuery(
    Guid TenantId,
    Guid UserId) : IRequest<Result<IReadOnlyList<ScheduleSummaryDto>>>;
