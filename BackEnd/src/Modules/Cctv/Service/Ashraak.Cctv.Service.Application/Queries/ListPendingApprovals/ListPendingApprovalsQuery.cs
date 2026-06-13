using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.ListPendingApprovals;

public sealed record ListPendingApprovalsQuery(
    Guid TenantId,
    Guid UserId) : IRequest<Result<IReadOnlyList<VisitSummaryDto>>>;
