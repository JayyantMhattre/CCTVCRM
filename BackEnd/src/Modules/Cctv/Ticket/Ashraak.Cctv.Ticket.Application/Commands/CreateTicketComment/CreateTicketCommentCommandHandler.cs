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

namespace Ashraak.Cctv.Ticket.Application.Commands.CreateTicketComment;

internal sealed class CreateTicketCommentCommandHandler(
    ITicketRepository ticketRepository,
    ISiteLookupService siteLookup,
    IEngineerLookupService engineerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTicketCommentCommand, Result<TicketCommentDto>>
{
    public async Task<Result<TicketCommentDto>> Handle(
        CreateTicketCommentCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.TicketsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Tickets.Disabled", "Ticket management is not enabled for this tenant.");

        var authError = await TicketAuthorization.EnsureCanUpdateAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var ticket = await ticketRepository.GetByIdAsync(TicketId.From(request.TicketId), cancellationToken);
        if (ticket is null)
            return Error.NotFound("Tickets.NotFound", "Ticket not found.");

        var accessError = await EnsureTicketAccessAsync(
            ticket, request.UserId, request.TenantId, permissionChecker, siteLookup, engineerLookup, cancellationToken);
        if (accessError is not null)
            return accessError;

        var isAdmin = await TicketAuthorization.IsAdminAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        var isEngineer = await TicketAuthorization.IsEngineerAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        var isCustomer = await TicketAuthorization.IsCustomerAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);

        var authorRole = TicketMapper.ResolveAuthorRole(isAdmin, isEngineer, isCustomer);

        try
        {
            var commentId = TicketCommentId.New();
            ticket.AddComment(commentId, request.Text, authorRole, request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var comment = ticket.Comments.First(c => c.Id == commentId);
            return TicketMapper.ToComment(comment);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Tickets.CommentFailed", ex.Message);
        }
    }

    internal static async Task<Error?> EnsureTicketAccessAsync(
        Domain.Aggregates.Ticket.Ticket ticket,
        Guid userId,
        Guid tenantId,
        IAuthPermissionChecker permissionChecker,
        ISiteLookupService siteLookup,
        IEngineerLookupService engineerLookup,
        CancellationToken cancellationToken)
    {
        if (await TicketAuthorization.IsAdminAsync(permissionChecker, userId, tenantId, cancellationToken))
            return null;

        if (await TicketAuthorization.IsCustomerAsync(permissionChecker, userId, tenantId, cancellationToken))
        {
            if (await siteLookup.ValidateSiteOwnershipAsync(ticket.SiteId, userId, cancellationToken))
                return null;

            return Error.Forbidden("Tickets.AccessDenied", "You do not have access to this ticket.");
        }

        if (await TicketAuthorization.IsEngineerAsync(permissionChecker, userId, tenantId, cancellationToken))
        {
            var engineer = await engineerLookup.GetForPlatformUserAsync(userId, cancellationToken);
            if (engineer is not null && ticket.ActiveAssignment?.EngineerId == engineer.Id)
                return null;

            return Error.Forbidden("Tickets.AccessDenied", "You do not have access to this ticket.");
        }

        return Error.Forbidden("Tickets.AccessDenied", "You do not have access to this ticket.");
    }
}
