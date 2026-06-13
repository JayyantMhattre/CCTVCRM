using Ashraak.Cctv.Ticket.Application.Commands.CreateTicketComment;
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

namespace Ashraak.Cctv.Ticket.Application.Commands.LinkTicketAttachment;

internal sealed class LinkTicketAttachmentCommandHandler(
    ITicketRepository ticketRepository,
    ISiteLookupService siteLookup,
    IEngineerLookupService engineerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<LinkTicketAttachmentCommand, Result<TicketAttachmentDto>>
{
    public async Task<Result<TicketAttachmentDto>> Handle(
        LinkTicketAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.TicketsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Tickets.Disabled", "Ticket management is not enabled for this tenant.");

        var authError = await TicketAuthorization.EnsureCanCreateAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var ticket = await ticketRepository.GetByIdAsync(TicketId.From(request.TicketId), cancellationToken);
        if (ticket is null)
            return Error.NotFound("Tickets.NotFound", "Ticket not found.");

        var accessError = await CreateTicketCommentCommandHandler.EnsureTicketAccessAsync(
            ticket, request.UserId, request.TenantId, permissionChecker, siteLookup, engineerLookup, cancellationToken);
        if (accessError is not null)
            return accessError;

        try
        {
            var attachmentId = TicketAttachmentId.New();
            var attachment = ticket.LinkAttachment(attachmentId, request.FileId, request.Title, request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return TicketMapper.ToAttachment(attachment);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Tickets.AttachmentFailed", ex.Message);
        }
    }
}
