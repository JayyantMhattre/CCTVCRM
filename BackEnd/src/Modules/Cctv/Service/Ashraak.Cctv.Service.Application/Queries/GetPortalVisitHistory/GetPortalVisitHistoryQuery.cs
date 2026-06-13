using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetPortalVisitHistory;

public sealed record GetPortalVisitHistoryQuery(
    Guid TenantId,
    Guid UserId) : IRequest<Result<IReadOnlyList<VisitSummaryDto>>>;
