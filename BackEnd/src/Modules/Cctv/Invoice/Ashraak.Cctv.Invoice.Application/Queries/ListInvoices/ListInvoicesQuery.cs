using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Queries.ListInvoices;

public sealed record ListInvoicesQuery(
    Guid TenantId,
    Guid UserId,
    int PageNumber,
    int PageSize,
    string? Status,
    string? InvoiceType,
    Guid? CustomerId,
    string? Search) : IRequest<Result<PagedResult<InvoiceSummaryDto>>>;
