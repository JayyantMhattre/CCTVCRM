using Ashraak.Cctv.Customer.Application.Commands.ChangeSiteStatus;
using Ashraak.Cctv.Customer.Application.Commands.CreateSite;
using Ashraak.Cctv.Customer.Application.Commands.LinkSiteDocument;
using Ashraak.Cctv.Customer.Application.Commands.RemoveSiteDocument;
using Ashraak.Cctv.Customer.Application.Commands.UpdateSite;
using Ashraak.Cctv.Customer.Application.Commands.UpdateSiteAssetSummary;
using Ashraak.Cctv.Customer.Application.Commands.UpsertSiteContacts;
using Ashraak.Cctv.Customer.Application.Queries.GetPortalSiteDetail;
using Ashraak.Cctv.Customer.Application.Queries.GetPortalSites;
using Ashraak.Cctv.Customer.Application.Queries.GetSite;
using Ashraak.Cctv.Customer.Application.Queries.GetSiteAssetSummary;
using Ashraak.Cctv.Customer.Application.Queries.GetSiteContacts;
using Ashraak.Cctv.Customer.Application.Queries.GetSiteDocuments;
using Ashraak.Cctv.Customer.Application.Queries.ListSites;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Cctv.Customer.Api.Endpoints;

/// <summary>Site management endpoints (D1-3) under <c>/api/v1/cctv</c>.</summary>
public static class CctvSiteEndpoints
{
    public static IEndpointRouteBuilder MapCctvSiteEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/cctv/sites")
            .WithTags("CCTV — Site")
            .RequireAuthorization();

        group.MapGet("/", ListSites)
            .WithName("ListCctvSites")
            .WithSummary("Paginated site list.");

        group.MapGet("/{siteId:guid}", GetSite)
            .WithName("GetCctvSite")
            .WithSummary("Get site detail.");

        group.MapPost("/", CreateSite)
            .WithName("CreateCctvSite")
            .WithSummary("Create a site for a customer.");

        group.MapPut("/{siteId:guid}", UpdateSite)
            .WithName("UpdateCctvSite")
            .WithSummary("Update site fields.");

        group.MapPatch("/{siteId:guid}/status", ChangeSiteStatus)
            .WithName("ChangeCctvSiteStatus")
            .WithSummary("Activate or deactivate a site.");

        group.MapGet("/{siteId:guid}/contacts", GetSiteContacts)
            .WithName("GetCctvSiteContacts")
            .WithSummary("List site contacts (max 3).");

        group.MapPut("/{siteId:guid}/contacts", UpsertSiteContacts)
            .WithName("UpsertCctvSiteContacts")
            .WithSummary("Replace site contact set.");

        group.MapGet("/{siteId:guid}/documents", GetSiteDocuments)
            .WithName("GetCctvSiteDocuments")
            .WithSummary("List site documents.");

        group.MapPost("/{siteId:guid}/documents", LinkSiteDocument)
            .WithName("LinkCctvSiteDocument")
            .WithSummary("Link a document to a site.");

        group.MapDelete("/{siteId:guid}/documents/{documentId:guid}", RemoveSiteDocument)
            .WithName("RemoveCctvSiteDocument")
            .WithSummary("Remove a site document link.");

        group.MapGet("/{siteId:guid}/asset-summary", GetSiteAssetSummary)
            .WithName("GetCctvSiteAssetSummary")
            .WithSummary("Get site asset summary.");

        group.MapPut("/{siteId:guid}/asset-summary", UpdateSiteAssetSummary)
            .WithName("UpdateCctvSiteAssetSummary")
            .WithSummary("Update site asset summary counts.");

        var portal = routeBuilder.MapGroup("/cctv/portal")
            .WithTags("CCTV — Customer Portal")
            .RequireAuthorization();

        portal.MapGet("/sites", GetPortalSites)
            .WithName("GetCctvPortalSites")
            .WithSummary("List own sites (Customer role).");

        portal.MapGet("/sites/{siteId:guid}", GetPortalSiteDetail)
            .WithName("GetCctvPortalSiteDetail")
            .WithSummary("Get own site detail (Customer role).");

        return routeBuilder;
    }

    private static async Task<IResult> ListSites(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] Guid? customerId,
        [FromQuery] string? status,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ListSitesQuery(
                tenantContext.TenantId,
                currentUser.UserId,
                page < 1 ? 1 : page,
                pageSize < 1 ? 20 : pageSize,
                customerId,
                status,
                search),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetSite(
        Guid siteId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetSiteQuery(tenantContext.TenantId, currentUser.UserId, siteId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreateSite(
        [FromBody] CreateSiteRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateSiteCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                request.CustomerId,
                request.Name,
                request.Address,
                request.City),
            cancellationToken);

        return ToResult(result, value => Results.Created($"/api/v1/cctv/sites/{value.Id}", value));
    }

    private static async Task<IResult> UpdateSite(
        Guid siteId,
        [FromBody] UpdateSiteRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateSiteCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                siteId,
                request.Name,
                request.Address,
                request.City,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> ChangeSiteStatus(
        Guid siteId,
        [FromBody] ChangeSiteStatusRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ChangeSiteStatusCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                siteId,
                request.Status,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetSiteContacts(
        Guid siteId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetSiteContactsQuery(tenantContext.TenantId, currentUser.UserId, siteId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> UpsertSiteContacts(
        Guid siteId,
        [FromBody] UpsertSiteContactsRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpsertSiteContactsCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                siteId,
                request.Contacts,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetSiteDocuments(
        Guid siteId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetSiteDocumentsQuery(tenantContext.TenantId, currentUser.UserId, siteId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> LinkSiteDocument(
        Guid siteId,
        [FromBody] LinkSiteDocumentRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LinkSiteDocumentCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                siteId,
                request.FileId,
                request.DocumentType,
                request.Title,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, value => Results.Created($"/api/v1/cctv/sites/{siteId}/documents/{value.Id}", value));
    }

    private static async Task<IResult> RemoveSiteDocument(
        Guid siteId,
        Guid documentId,
        [FromQuery] uint rowVersion,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RemoveSiteDocumentCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                siteId,
                documentId,
                rowVersion),
            cancellationToken);

        return ToVoidResult(result);
    }

    private static async Task<IResult> GetSiteAssetSummary(
        Guid siteId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetSiteAssetSummaryQuery(tenantContext.TenantId, currentUser.UserId, siteId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> UpdateSiteAssetSummary(
        Guid siteId,
        [FromBody] UpdateSiteAssetSummaryRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateSiteAssetSummaryCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                siteId,
                request.CameraCount,
                request.DvrCount,
                request.NvrCount,
                request.HardDiskCount,
                request.SwitchCount,
                request.RouterCount,
                request.MonitorCount,
                request.Brand,
                request.Model,
                request.Remarks,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetPortalSites(
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetPortalSitesQuery(tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetPortalSiteDetail(
        Guid siteId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetPortalSiteDetailQuery(tenantContext.TenantId, currentUser.UserId, siteId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static IResult ToResult<T>(Result<T> result, Func<T, IResult> onSuccess)
    {
        if (result.IsSuccess)
            return onSuccess(result.Value);

        return ToErrorResult(result.Error);
    }

    private static IResult ToVoidResult(Result result)
    {
        if (result.IsSuccess)
            return Results.NoContent();

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
