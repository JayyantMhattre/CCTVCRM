using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.Cctv.Ticket.Application.Mapping;
using Ashraak.Cctv.Ticket.Domain.Enums;
using Ashraak.Cctv.Ticket.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Queries.ListTickets;

internal sealed class ListTicketsQueryHandler(
    ITicketRepository ticketRepository,
    ICustomerLookupService customerLookup,
    IEngineerLookupService engineerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<ListTicketsQuery, Result<PagedResult<TicketSummaryDto>>>
{
    public async Task<Result<PagedResult<TicketSummaryDto>>> Handle(
        ListTicketsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.TicketsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Tickets.Disabled", "Ticket management is not enabled for this tenant.");

        var authError = await TicketAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        TicketStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!TicketMapper.TryParseStatus(request.Status, out var parsedStatus))
                return Error.Validation("Tickets.InvalidStatus", "Invalid status filter.");

            status = parsedStatus;
        }

        TicketPriority? priority = null;
        if (!string.IsNullOrWhiteSpace(request.Priority))
        {
            if (!TicketMapper.TryParsePriority(request.Priority, out var parsedPriority))
                return Error.Validation("Tickets.InvalidPriority", "Invalid priority filter.");

            priority = parsedPriority;
        }

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, 100);

        var isAdmin = await TicketAuthorization.IsAdminAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);

        Guid? customerId = request.CustomerId;
        Guid? engineerId = request.EngineerId;

        if (!isAdmin)
        {
            if (await TicketAuthorization.IsCustomerAsync(permissionChecker, request.UserId, request.TenantId, cancellationToken))
            {
                var customer = await customerLookup.GetCustomerForUserAsync(request.UserId, cancellationToken);
                if (customer is null)
                    return new PagedResult<TicketSummaryDto>([], pageNumber, pageSize, 0);

                customerId = customer.Id;
            }
            else if (await TicketAuthorization.IsEngineerAsync(permissionChecker, request.UserId, request.TenantId, cancellationToken))
            {
                var engineer = await engineerLookup.GetForPlatformUserAsync(request.UserId, cancellationToken);
                if (engineer is null)
                    return new PagedResult<TicketSummaryDto>([], pageNumber, pageSize, 0);

                engineerId = engineer.Id;
            }
        }

        var result = await ticketRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            status,
            priority,
            customerId,
            request.SiteId,
            engineerId,
            request.Search,
            cancellationToken);

        var items = result.Items.Select(TicketMapper.ToSummary).ToList();
        return new PagedResult<TicketSummaryDto>(items, pageNumber, pageSize, result.TotalCount);
    }
}
