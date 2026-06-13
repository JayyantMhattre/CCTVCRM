using Ashraak.Cctv.Lead.Application.Commands.ChangeLeadStatus;
using Ashraak.Cctv.Lead.Application.Commands.ConvertLead;
using Ashraak.Cctv.Lead.Application.Commands.CreateInquiry;
using Ashraak.Cctv.Lead.Application.Commands.CreateLead;
using Ashraak.Cctv.Lead.Application.Commands.CreateLeadActivity;
using Ashraak.Cctv.Lead.Application.Commands.CreateLeadRemark;
using Ashraak.Cctv.Lead.Application.Commands.LinkLeadAttachment;
using Ashraak.Cctv.Lead.Application.Commands.RemoveLeadAttachment;
using Ashraak.Cctv.Lead.Application.Commands.UpdateLead;
using Ashraak.Cctv.Lead.Application.Queries.GetLead;
using Ashraak.Cctv.Lead.Application.Queries.GetLeadActivities;
using Ashraak.Cctv.Lead.Application.Queries.GetLeadAttachments;
using Ashraak.Cctv.Lead.Application.Queries.GetLeadRemarks;
using Ashraak.Cctv.Lead.Application.Queries.ListLeads;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Cctv.Lead.Api.Endpoints;

/// <summary>Lead management endpoints (B1) under <c>/api/v1/cctv</c>.</summary>
public static class CctvLeadEndpoints
{
    public static IEndpointRouteBuilder MapCctvLeadEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/cctv/inquiries", CreateInquiry)
            .WithTags("CCTV — Lead")
            .WithName("CreateCctvInquiry")
            .WithSummary("Submit a public website inquiry (anonymous).")
            .AllowAnonymous();

        var group = routeBuilder.MapGroup("/cctv/leads")
            .WithTags("CCTV — Lead")
            .RequireAuthorization();

        group.MapGet("/", ListLeads)
            .WithName("ListCctvLeads")
            .WithSummary("Paginated lead pipeline list.");

        group.MapGet("/{leadId:guid}", GetLead)
            .WithName("GetCctvLead")
            .WithSummary("Get lead detail with child counts.");

        group.MapPost("/", CreateLead)
            .WithName("CreateCctvLead")
            .WithSummary("Create a manual lead.");

        group.MapPut("/{leadId:guid}", UpdateLead)
            .WithName("UpdateCctvLead")
            .WithSummary("Update mutable lead fields.");

        group.MapPost("/{leadId:guid}/status", ChangeLeadStatus)
            .WithName("ChangeCctvLeadStatus")
            .WithSummary("Transition lead pipeline status.");

        group.MapGet("/{leadId:guid}/activities", GetLeadActivities)
            .WithName("GetCctvLeadActivities")
            .WithSummary("Get lead activity timeline.");

        group.MapPost("/{leadId:guid}/activities", CreateLeadActivity)
            .WithName("CreateCctvLeadActivity")
            .WithSummary("Log a lead activity.");

        group.MapGet("/{leadId:guid}/remarks", GetLeadRemarks)
            .WithName("GetCctvLeadRemarks")
            .WithSummary("Get lead remarks.");

        group.MapPost("/{leadId:guid}/remarks", CreateLeadRemark)
            .WithName("CreateCctvLeadRemark")
            .WithSummary("Add a lead remark.");

        group.MapGet("/{leadId:guid}/attachments", GetLeadAttachments)
            .WithName("GetCctvLeadAttachments")
            .WithSummary("Get lead attachment metadata.");

        group.MapPost("/{leadId:guid}/attachments", LinkLeadAttachment)
            .WithName("LinkCctvLeadAttachment")
            .WithSummary("Link a platform file to a lead.");

        group.MapDelete("/{leadId:guid}/attachments/{attachmentId:guid}", RemoveLeadAttachment)
            .WithName("RemoveCctvLeadAttachment")
            .WithSummary("Remove a lead attachment link.");

        group.MapPost("/{leadId:guid}/convert", ConvertLead)
            .WithName("ConvertCctvLead")
            .WithSummary("Convert a Won lead to Customer + Site + AMC contract.");

        return routeBuilder;
    }

    private static async Task<IResult> CreateInquiry(
        [FromBody] CreateInquiryRequest request,
        ITenantContext tenantContext,
        ICurrentUser currentUser,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.IsAuthenticated ? currentUser.UserId : Guid.Empty;
        var result = await sender.Send(
            new CreateInquiryCommand(
                tenantContext.TenantId,
                userId,
                request.InquiryType,
                request.Name,
                request.Organization,
                request.Email,
                request.Phone,
                request.City,
                request.Address,
                request.RequirementSummary,
                request.PreferredPlanCode,
                request.SourcePage),
            cancellationToken);

        return ToResult(result, value => Results.Created($"/api/v1/cctv/leads/{value.LeadId}", value));
    }

    private static async Task<IResult> ListLeads(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? status,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ListLeadsQuery(
                tenantContext.TenantId,
                currentUser.UserId,
                page < 1 ? 1 : page,
                pageSize < 1 ? 20 : pageSize,
                status,
                search),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetLead(
        Guid leadId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetLeadQuery(tenantContext.TenantId, currentUser.UserId, leadId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreateLead(
        [FromBody] CreateLeadRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateLeadCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                request.ContactName,
                request.OrganizationName,
                request.Email,
                request.Phone,
                request.City,
                request.Address,
                request.RequirementSummary,
                request.Source,
                request.OwnerUserId),
            cancellationToken);

        return ToResult(result, value => Results.Created($"/api/v1/cctv/leads/{value.Id}", value));
    }

    private static async Task<IResult> UpdateLead(
        Guid leadId,
        [FromBody] UpdateLeadRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateLeadCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                leadId,
                request.ContactName,
                request.OrganizationName,
                request.Email,
                request.Phone,
                request.City,
                request.Address,
                request.RequirementSummary,
                request.OwnerUserId,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> ChangeLeadStatus(
        Guid leadId,
        [FromBody] ChangeLeadStatusRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ChangeLeadStatusCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                leadId,
                request.ToStatus,
                request.Notes,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetLeadActivities(
        Guid leadId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetLeadActivitiesQuery(tenantContext.TenantId, currentUser.UserId, leadId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreateLeadActivity(
        Guid leadId,
        [FromBody] CreateLeadActivityRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateLeadActivityCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                leadId,
                request.ActivityType,
                request.Description,
                request.FromStatus,
                request.ToStatus),
            cancellationToken);

        return ToResult(result, activity => Results.Created($"/api/v1/cctv/leads/{leadId}/activities", activity));
    }

    private static async Task<IResult> GetLeadRemarks(
        Guid leadId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetLeadRemarksQuery(tenantContext.TenantId, currentUser.UserId, leadId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreateLeadRemark(
        Guid leadId,
        [FromBody] CreateLeadRemarkRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateLeadRemarkCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                leadId,
                request.Text),
            cancellationToken);

        return ToResult(result, value => Results.Created($"/api/v1/cctv/leads/{leadId}/remarks/{value.Id}", value));
    }

    private static async Task<IResult> GetLeadAttachments(
        Guid leadId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetLeadAttachmentsQuery(tenantContext.TenantId, currentUser.UserId, leadId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> LinkLeadAttachment(
        Guid leadId,
        [FromBody] LinkLeadAttachmentRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LinkLeadAttachmentCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                leadId,
                request.FileId,
                request.Title),
            cancellationToken);

        return ToResult(result, value => Results.Created($"/api/v1/cctv/leads/{leadId}/attachments/{value.Id}", value));
    }

    private static async Task<IResult> RemoveLeadAttachment(
        Guid leadId,
        Guid attachmentId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RemoveLeadAttachmentCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                leadId,
                attachmentId),
            cancellationToken);

        return ToVoidResult(result, () => Results.NoContent());
    }

    private static async Task<IResult> ConvertLead(
        Guid leadId,
        [FromBody] ConvertLeadRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ConvertLeadCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                leadId,
                request.PlanVersionId,
                request.SiteName,
                request.SiteAddress,
                request.InitialTermStartDate,
                request.InitialTermEndDate,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static IResult ToResult<T>(Result<T> result, Func<T, IResult> onSuccess)
    {
        if (result.IsSuccess)
            return onSuccess(result.Value);

        return ToErrorResult(result.Error);
    }

    private static IResult ToVoidResult(Result result, Func<IResult> onSuccess)
    {
        if (result.IsSuccess)
            return onSuccess();

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
