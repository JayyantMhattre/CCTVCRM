using Ashraak.Cctv.Ticket.Application.Mapping;
using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Ashraak.Cctv.Ticket.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Commands.ReopenTicket;

internal sealed class ReopenTicketCommandHandler(
    ITicketRepository ticketRepository,
    ISiteLookupService siteLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<ReopenTicketCommand, Result<TicketDetailDto>>
{
    public async Task<Result<TicketDetailDto>> Handle(
        ReopenTicketCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.TicketsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Tickets.Disabled", "Ticket management is not enabled for this tenant.");

        var authError = await TicketAuthorization.EnsureCanReopenAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var ticket = await ticketRepository.GetByIdAsync(TicketId.From(request.TicketId), cancellationToken);
        if (ticket is null)
            return Error.NotFound("Tickets.NotFound", "Ticket not found.");

        var isAdmin = await TicketAuthorization.IsAdminAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (!isAdmin)
        {
            if (!await siteLookup.ValidateSiteOwnershipAsync(ticket.SiteId, request.UserId, cancellationToken))
                return Error.Forbidden("Tickets.SiteAccessDenied", "You can only reopen tickets for your own sites.");
        }

        var concurrencyError = TicketConcurrencyHelper.EnsureRowVersion(request.RowVersion, ticket.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        try
        {
            ticket.Reopen(request.Reason, request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return TicketMapper.ToDetail(ticket);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Tickets.ReopenFailed", ex.Message);
        }
    }
}
