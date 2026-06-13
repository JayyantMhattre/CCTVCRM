using Ashraak.Cctv.Ticket.Application.Abstractions;
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
using TicketAggregate = Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket.Ticket;

namespace Ashraak.Cctv.Ticket.Application.Commands.CreateTicket;

internal sealed class CreateTicketCommandHandler(
    ITicketRepository ticketRepository,
    ITicketNumberGenerator numberGenerator,
    ISiteLookupService siteLookup,
    IAmcContractLookupService amcLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTicketCommand, Result<TicketDetailDto>>
{
    public async Task<Result<TicketDetailDto>> Handle(
        CreateTicketCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.TicketsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Tickets.Disabled", "Ticket management is not enabled for this tenant.");

        var authError = await TicketAuthorization.EnsureCanCreateAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var isAdmin = await TicketAuthorization.IsAdminAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        var isCustomer = await TicketAuthorization.IsCustomerAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        var isEngineer = await TicketAuthorization.IsEngineerAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);

        if (isCustomer && !isAdmin)
        {
            if (!await siteLookup.ValidateSiteOwnershipAsync(request.SiteId, request.UserId, cancellationToken))
                return Error.Forbidden("Tickets.SiteAccessDenied", "You can only create tickets for your own sites.");
        }

        if (!TicketMapper.TryParsePriority(request.Priority, out var priority))
            return Error.Validation("Tickets.InvalidPriority", "Invalid ticket priority.");

        var site = await siteLookup.GetSiteAsync(request.SiteId, cancellationToken);
        if (site is null)
            return Error.NotFound("Sites.NotFound", "Site not found.");

        var amcContract = await amcLookup.GetActiveContractForSiteAsync(request.SiteId, cancellationToken);

        TicketSource source;
        if (request.ServiceVisitId.HasValue)
            source = TicketSource.EngineerVisit;
        else if (isAdmin)
            source = TicketSource.Admin;
        else if (isEngineer)
            source = TicketSource.EngineerVisit;
        else
            source = TicketSource.Customer;

        try
        {
            var ticketNumber = await numberGenerator.GenerateNextAsync(cancellationToken);
            var ticket = TicketAggregate.Create(
                TicketId.New(),
                ticketNumber,
                site.CustomerId,
                request.SiteId,
                amcContract?.Id,
                request.ServiceVisitId,
                source,
                request.Subject,
                request.Description,
                priority,
                request.UserId);

            if (request.AttachmentFileIds is not null)
            {
                foreach (var fileId in request.AttachmentFileIds)
                    ticket.LinkAttachment(TicketAttachmentId.New(), fileId, null, request.UserId);
            }

            ticketRepository.Add(ticket);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return TicketMapper.ToDetail(ticket);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Tickets.CreateFailed", ex.Message);
        }
    }
}
