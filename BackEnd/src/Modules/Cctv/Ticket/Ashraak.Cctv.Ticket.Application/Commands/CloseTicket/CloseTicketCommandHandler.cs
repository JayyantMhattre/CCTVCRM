using Ashraak.Cctv.Ticket.Application.Mapping;
using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Ashraak.Cctv.Ticket.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Commands.CloseTicket;

internal sealed class CloseTicketCommandHandler(
    ITicketRepository ticketRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CloseTicketCommand, Result<TicketDetailDto>>
{
    public async Task<Result<TicketDetailDto>> Handle(
        CloseTicketCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.TicketsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Tickets.Disabled", "Ticket management is not enabled for this tenant.");

        var authError = await TicketAuthorization.EnsureCanCloseAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var ticket = await ticketRepository.GetByIdAsync(TicketId.From(request.TicketId), cancellationToken);
        if (ticket is null)
            return Error.NotFound("Tickets.NotFound", "Ticket not found.");

        var concurrencyError = TicketConcurrencyHelper.EnsureRowVersion(request.RowVersion, ticket.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        try
        {
            ticket.Close(request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return TicketMapper.ToDetail(ticket);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Tickets.CloseFailed", ex.Message);
        }
    }
}
