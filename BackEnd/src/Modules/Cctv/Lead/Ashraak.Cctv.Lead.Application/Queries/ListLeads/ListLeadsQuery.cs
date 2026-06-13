using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Queries.ListLeads;

public sealed record ListLeadsQuery(
    Guid TenantId,
    Guid UserId,
    int PageNumber,
    int PageSize,
    string? Status,
    string? Search) : IRequest<Result<PagedResult<LeadSummaryDto>>>;
