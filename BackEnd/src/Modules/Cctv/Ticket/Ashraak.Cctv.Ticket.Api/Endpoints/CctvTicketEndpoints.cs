using Ashraak.Cctv.Ticket.Application.Commands.AssignTicket;
using Ashraak.Cctv.Ticket.Application.Commands.CloseTicket;
using Ashraak.Cctv.Ticket.Application.Commands.CreateTicket;
using Ashraak.Cctv.Ticket.Application.Commands.CreateTicketComment;
using Ashraak.Cctv.Ticket.Application.Commands.LinkTicketAttachment;
using Ashraak.Cctv.Ticket.Application.Commands.ReopenTicket;
using Ashraak.Cctv.Ticket.Application.Commands.UpdateTicketStatus;
using Ashraak.Cctv.Ticket.Application.Queries.GetEngineerTickets;
using Ashraak.Cctv.Ticket.Application.Queries.GetPortalTickets;
using Ashraak.Cctv.Ticket.Application.Queries.GetTicket;
using Ashraak.Cctv.Ticket.Application.Queries.ListTickets;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Cctv.Ticket.Api.Endpoints;

/// <summary>Ticket management endpoints (D1-6) under <c>/api/v1/cctv</c>.</summary>
public static class CctvTicketEndpoints
{
    public static IEndpointRouteBuilder MapCctvTicketEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var tickets = routeBuilder.MapGroup("/cctv/tickets")
            .WithTags("CCTV — Ticket")
            .RequireAuthorization();

        tickets.MapGet("/", ListTickets)
            .WithName("ListCctvTickets")
            .WithSummary("Paginated ticket list.");

        tickets.MapGet("/{ticketId:guid}", GetTicket)
            .WithName("GetCctvTicket")
            .WithSummary("Get ticket detail.");

        tickets.MapPost("/", CreateTicket)
            .WithName("CreateCctvTicket")
            .WithSummary("Create a new ticket.");

        tickets.MapPost("/{ticketId:guid}/assign", AssignTicket)
            .WithName("AssignCctvTicket")
            .WithSummary("Assign engineer to ticket.");

        tickets.MapPatch("/{ticketId:guid}/status", UpdateTicketStatus)
            .WithName("UpdateCctvTicketStatus")
            .WithSummary("Update ticket status.");

        tickets.MapPost("/{ticketId:guid}/comments", CreateTicketComment)
            .WithName("CreateCctvTicketComment")
            .WithSummary("Add comment to ticket.");

        tickets.MapPost("/{ticketId:guid}/attachments", LinkTicketAttachment)
            .WithName("LinkCctvTicketAttachment")
            .WithSummary("Link attachment to ticket.");

        tickets.MapPost("/{ticketId:guid}/close", CloseTicket)
            .WithName("CloseCctvTicket")
            .WithSummary("Close resolved ticket.");

        tickets.MapPost("/{ticketId:guid}/reopen", ReopenTicket)
            .WithName("ReopenCctvTicket")
            .WithSummary("Reopen closed ticket.");

        routeBuilder.MapGet("/cctv/portal/tickets", GetPortalTickets)
            .WithTags("CCTV — Customer Portal")
            .RequireAuthorization()
            .WithName("GetCctvPortalTickets")
            .WithSummary("Customer portal ticket list.");

        routeBuilder.MapGet("/cctv/engineer/tickets", GetEngineerTickets)
            .WithTags("CCTV — Engineer Portal")
            .RequireAuthorization()
            .WithName("GetCctvEngineerTickets")
            .WithSummary("Engineer assigned ticket queue.");

        return routeBuilder;
    }

    private static async Task<IResult> ListTickets(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? siteId,
        [FromQuery] Guid? engineerId,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ListTicketsQuery(
                tenantContext.TenantId,
                currentUser.UserId,
                page < 1 ? 1 : page,
                pageSize < 1 ? 20 : pageSize,
                status,
                priority,
                customerId,
                siteId,
                engineerId,
                search),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetTicket(
        Guid ticketId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetTicketQuery(tenantContext.TenantId, currentUser.UserId, ticketId),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreateTicket(
        [FromBody] CreateTicketRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateTicketCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                request.SiteId,
                request.Subject,
                request.Description,
                request.Priority,
                request.ServiceVisitId,
                request.AttachmentFileIds),
            cancellationToken);
        return ToResult(result, value => Results.Created($"/api/v1/cctv/tickets/{value.Id}", value));
    }

    private static async Task<IResult> AssignTicket(
        Guid ticketId,
        [FromBody] AssignTicketRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AssignTicketCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                ticketId,
                request.EngineerId,
                request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> UpdateTicketStatus(
        Guid ticketId,
        [FromBody] UpdateTicketStatusRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateTicketStatusCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                ticketId,
                request.ToStatus,
                request.Comment,
                request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreateTicketComment(
        Guid ticketId,
        [FromBody] CreateTicketCommentRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateTicketCommentCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                ticketId,
                request.Text),
            cancellationToken);
        return ToResult(result, value => Results.Created($"/api/v1/cctv/tickets/{ticketId}/comments/{value.Id}", value));
    }

    private static async Task<IResult> LinkTicketAttachment(
        Guid ticketId,
        [FromBody] LinkTicketAttachmentRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LinkTicketAttachmentCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                ticketId,
                request.FileId,
                request.Title),
            cancellationToken);
        return ToResult(result, value => Results.Created($"/api/v1/cctv/tickets/{ticketId}/attachments/{value.Id}", value));
    }

    private static async Task<IResult> CloseTicket(
        Guid ticketId,
        [FromBody] CloseTicketRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CloseTicketCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                ticketId,
                request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> ReopenTicket(
        Guid ticketId,
        [FromBody] ReopenTicketRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ReopenTicketCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                ticketId,
                request.Reason,
                request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetPortalTickets(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetPortalTicketsQuery(
                tenantContext.TenantId,
                currentUser.UserId,
                page < 1 ? 1 : page,
                pageSize < 1 ? 20 : pageSize),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetEngineerTickets(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetEngineerTicketsQuery(
                tenantContext.TenantId,
                currentUser.UserId,
                page < 1 ? 1 : page,
                pageSize < 1 ? 20 : pageSize),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static IResult ToResult<T>(Result<T> result, Func<T, IResult> onSuccess)
    {
        if (result.IsSuccess)
            return onSuccess(result.Value);

        return ToErrorResult(result.Error);
    }

    private static IResult ToErrorResult(Error error) =>
        error.Type switch
        {
            ErrorType.NotFound => Results.NotFound(),
            ErrorType.Forbidden or ErrorType.Unauthorized
                => Results.Problem(error.Description, statusCode: StatusCodes.Status403Forbidden),
            ErrorType.Conflict
                => Results.Problem(error.Description, statusCode: StatusCodes.Status409Conflict),
            _ => Results.Problem(error.Description, statusCode: StatusCodes.Status400BadRequest)
        };
}
