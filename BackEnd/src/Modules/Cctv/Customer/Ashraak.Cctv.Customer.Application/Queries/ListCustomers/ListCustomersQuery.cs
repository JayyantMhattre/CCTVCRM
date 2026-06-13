using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.ListCustomers;

public sealed record ListCustomersQuery(
    Guid TenantId,
    Guid UserId,
    int PageNumber,
    int PageSize,
    string? Status,
    string? Search) : IRequest<Result<PagedResult<CustomerSummaryDto>>>;
