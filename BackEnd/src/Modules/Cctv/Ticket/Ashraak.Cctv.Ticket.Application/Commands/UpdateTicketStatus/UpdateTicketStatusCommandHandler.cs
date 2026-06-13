using Ashraak.Cctv.Ticket.Application.Mapping;
using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Ashraak.Cctv.Ticket.Domain.Enums;
using Ashraak.Cctv.Ticket.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Commands.UpdateTicketStatus;

internal sealed class UpdateTicketStatusCommandHandler(
    ITicketRepository ticketRepository,
    IEngineerLookupService engineerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTicketStatusCommand, Result<TicketDetailDto>>
{
    public async Task<Result<TicketDetailDto>> Handle(
        UpdateTicketStatusCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.TicketsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Tickets.Disabled", "Ticket management is not enabled for this tenant.");

        var authError = await TicketAuthorization.EnsureCanUpdateAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        if (!TicketMapper.TryParseStatus(request.ToStatus, out var toStatus))
            return Error.Validation("Tickets.InvalidStatus", "Invalid ticket status.");

        var ticket = await ticketRepository.GetByIdAsync(TicketId.From(request.TicketId), cancellationToken);
        if (ticket is null)
            return Error.NotFound("Tickets.NotFound", "Ticket not found.");

        var concurrencyError = TicketConcurrencyHelper.EnsureRowVersion(request.RowVersion, ticket.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        var isAdmin = await TicketAuthorization.IsAdminAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        var isEngineer = await TicketAuthorization.IsEngineerAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);

        if (!isAdmin && !isEngineer)
            return Error.Forbidden("Tickets.UpdateForbidden", "Only admin or assigned engineer can update status.");

        if (isEngineer && !isAdmin)
        {
            var engineer = await engineerLookup.GetForPlatformUserAsync(request.UserId, cancellationToken);
            if (engineer is null)
                return Error.Forbidden("Tickets.EngineerNotFound", "No engineer profile linked to this user.");

            var assignment = ticket.ActiveAssignment;
            if (assignment is null || assignment.EngineerId != engineer.Id)
                return Error.Forbidden("Tickets.NotAssigned", "You are not assigned to this ticket.");
        }

        var actorRole = TicketMapper.ResolveAuthorRole(isAdmin, isEngineer, false);

        try
        {
            ticket.UpdateStatus(toStatus, actorRole, request.UserId, request.Comment);

            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                ticket.AddComment(
                    TicketCommentId.New(),
                    request.Comment,
                    actorRole,
                    request.UserId);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return TicketMapper.ToDetail(ticket);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Tickets.StatusUpdateFailed", ex.Message);
        }
    }

}
