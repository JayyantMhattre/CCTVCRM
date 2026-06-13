using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Queries.GetPortalInvoices;

public sealed record GetPortalInvoicesQuery(
    Guid TenantId,
    Guid UserId,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedResult<InvoiceSummaryDto>>>;
