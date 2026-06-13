using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.Cctv.Ticket.Application.Mapping;
using Ashraak.Cctv.Ticket.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Queries.GetPortalTickets;

internal sealed class GetPortalTicketsQueryHandler(
    ITicketRepository ticketRepository,
    ICustomerLookupService customerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetPortalTicketsQuery, Result<PagedResult<TicketSummaryDto>>>
{
    public async Task<Result<PagedResult<TicketSummaryDto>>> Handle(
        GetPortalTicketsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.TicketsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Tickets.Disabled", "Ticket management is not enabled for this tenant.");

        if (!await TicketAuthorization.IsCustomerAsync(permissionChecker, request.UserId, request.TenantId, cancellationToken)
            && !await TicketAuthorization.IsAdminAsync(permissionChecker, request.UserId, request.TenantId, cancellationToken))
        {
            return Error.Forbidden("Tickets.PortalForbidden", "Customer portal access required.");
        }

        var customer = await customerLookup.GetCustomerForUserAsync(request.UserId, cancellationToken);
        if (customer is null)
            return new PagedResult<TicketSummaryDto>([], 1, 20, 0);

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, 100);

        var result = await ticketRepository.GetForCustomerAsync(
            customer.Id, pageNumber, pageSize, cancellationToken);

        var items = result.Items.Select(TicketMapper.ToSummary).ToList();
        return new PagedResult<TicketSummaryDto>(items, pageNumber, pageSize, result.TotalCount);
    }
}
