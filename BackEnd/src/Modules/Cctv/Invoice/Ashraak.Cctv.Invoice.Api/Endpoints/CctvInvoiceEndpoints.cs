using Ashraak.Cctv.Invoice.Application.Commands.CancelInvoice;
using Ashraak.Cctv.Invoice.Application.Commands.CreateInvoice;
using Ashraak.Cctv.Invoice.Application.Commands.GenerateInvoice;
using Ashraak.Cctv.Invoice.Application.Commands.MarkInvoicePaid;
using Ashraak.Cctv.Invoice.Application.Commands.SendInvoice;
using Ashraak.Cctv.Invoice.Application.Commands.UpdateInvoiceDraft;
using Ashraak.Cctv.Invoice.Application.Queries.GetInvoice;
using Ashraak.Cctv.Invoice.Application.Queries.GetInvoicePdf;
using Ashraak.Cctv.Invoice.Application.Queries.GetPortalInvoices;
using Ashraak.Cctv.Invoice.Application.Queries.ListInvoices;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Cctv.Invoice.Api.Endpoints;

/// <summary>Invoice management endpoints (D1-7, Option B) under <c>/api/v1/cctv</c>.</summary>
public static class CctvInvoiceEndpoints
{
    public static IEndpointRouteBuilder MapCctvInvoiceEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var invoices = routeBuilder.MapGroup("/cctv/invoices")
            .WithTags("CCTV — Invoice")
            .RequireAuthorization();

        invoices.MapGet("/", ListInvoices)
            .WithName("ListCctvInvoices")
            .WithSummary("Paginated invoice list.");

        invoices.MapGet("/{invoiceId:guid}", GetInvoice)
            .WithName("GetCctvInvoice")
            .WithSummary("Get invoice detail.");

        invoices.MapPost("/", CreateInvoice)
            .WithName("CreateCctvInvoice")
            .WithSummary("Create draft invoice.");

        invoices.MapPut("/{invoiceId:guid}", UpdateInvoiceDraft)
            .WithName("UpdateCctvInvoiceDraft")
            .WithSummary("Update draft invoice and lines.");

        invoices.MapPost("/{invoiceId:guid}/generate", GenerateInvoice)
            .WithName("GenerateCctvInvoice")
            .WithSummary("Generate invoice PDF and transition to Generated.");

        invoices.MapPost("/{invoiceId:guid}/send", SendInvoice)
            .WithName("SendCctvInvoice")
            .WithSummary("Mark invoice as sent.");

        invoices.MapPost("/{invoiceId:guid}/mark-paid", MarkInvoicePaid)
            .WithName("MarkCctvInvoicePaid")
            .WithSummary("Manually mark invoice as paid.");

        invoices.MapPost("/{invoiceId:guid}/cancel", CancelInvoice)
            .WithName("CancelCctvInvoice")
            .WithSummary("Cancel invoice.");

        invoices.MapGet("/{invoiceId:guid}/pdf", GetInvoicePdf)
            .WithName("GetCctvInvoicePdf")
            .WithSummary("Get invoice PDF download metadata.");

        routeBuilder.MapGet("/cctv/portal/invoices", GetPortalInvoices)
            .WithTags("CCTV — Customer Portal")
            .RequireAuthorization()
            .WithName("GetCctvPortalInvoices")
            .WithSummary("Customer portal invoice list.");

        return routeBuilder;
    }

    private static async Task<IResult> ListInvoices(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? status,
        [FromQuery] string? invoiceType,
        [FromQuery] Guid? customerId,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ListInvoicesQuery(
                tenantContext.TenantId,
                currentUser.UserId,
                page < 1 ? 1 : page,
                pageSize < 1 ? 20 : pageSize,
                status,
                invoiceType,
                customerId,
                search),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetInvoice(
        Guid invoiceId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetInvoiceQuery(tenantContext.TenantId, currentUser.UserId, invoiceId),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreateInvoice(
        [FromBody] CreateInvoiceRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateInvoiceCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                request.CustomerId,
                request.SiteId,
                request.InvoiceType,
                request.AmcContractTermId,
                request.TicketId,
                request.ServiceVisitId,
                request.InvoiceDate,
                request.DueDate,
                request.Lines,
                request.TaxAmount),
            cancellationToken);
        return ToResult(result, value => Results.Created($"/api/v1/cctv/invoices/{value.Id}", value));
    }

    private static async Task<IResult> UpdateInvoiceDraft(
        Guid invoiceId,
        [FromBody] UpdateInvoiceDraftRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateInvoiceDraftCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                invoiceId,
                request.SiteId,
                request.InvoiceType,
                request.AmcContractTermId,
                request.TicketId,
                request.ServiceVisitId,
                request.InvoiceDate,
                request.DueDate,
                request.Lines,
                request.TaxAmount,
                request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GenerateInvoice(
        Guid invoiceId,
        [FromBody] GenerateInvoiceRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GenerateInvoiceCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                invoiceId,
                request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> SendInvoice(
        Guid invoiceId,
        [FromBody] SendInvoiceRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new SendInvoiceCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                invoiceId,
                request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> MarkInvoicePaid(
        Guid invoiceId,
        [FromBody] MarkInvoicePaidRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new MarkInvoicePaidCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                invoiceId,
                request.PaidAt,
                request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CancelInvoice(
        Guid invoiceId,
        [FromBody] CancelInvoiceRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CancelInvoiceCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                invoiceId,
                request.Reason,
                request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetInvoicePdf(
        Guid invoiceId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetInvoicePdfQuery(tenantContext.TenantId, currentUser.UserId, invoiceId),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetPortalInvoices(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetPortalInvoicesQuery(
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
