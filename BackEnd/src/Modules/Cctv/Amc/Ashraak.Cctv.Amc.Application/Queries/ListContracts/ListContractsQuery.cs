using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.ListContracts;

public sealed record ListContractsQuery(
    Guid TenantId,
    Guid UserId,
    int Page,
    int PageSize,
    string? Status,
    Guid? SiteId,
    Guid? CustomerId,
    string? Search) : IRequest<Result<PagedResult<AmcContractSummaryDto>>>;
