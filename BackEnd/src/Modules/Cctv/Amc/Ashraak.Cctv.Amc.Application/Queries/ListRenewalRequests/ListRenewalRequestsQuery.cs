using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.ListRenewalRequests;

public sealed record ListRenewalRequestsQuery(
    Guid TenantId,
    Guid UserId,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<AmcRenewalRequestSummaryDto>>>;
