using Ashraak.Cctv.Service.Application.Commands.ApproveVisit;
using Ashraak.Cctv.Service.Application.Commands.AssignEngineer;
using Ashraak.Cctv.Service.Application.Commands.CancelSchedule;
using Ashraak.Cctv.Service.Application.Commands.CaptureVisitLocation;
using Ashraak.Cctv.Service.Application.Commands.CreateAdHocSchedule;
using Ashraak.Cctv.Service.Application.Commands.LinkVisitAttachment;
using Ashraak.Cctv.Service.Application.Commands.LinkVisitPhoto;
using Ashraak.Cctv.Service.Application.Commands.LinkVisitSelfie;
using Ashraak.Cctv.Service.Application.Commands.LinkVisitSignature;
using Ashraak.Cctv.Service.Application.Commands.OfflineVisitSyncBatch;
using Ashraak.Cctv.Service.Application.Commands.RescheduleSchedule;
using Ashraak.Cctv.Service.Application.Commands.ReturnVisit;
using Ashraak.Cctv.Service.Application.Commands.StartVisit;
using Ashraak.Cctv.Service.Application.Commands.SubmitVisitReport;
using Ashraak.Cctv.Service.Application.Commands.UpdateVisitRemarks;
using Ashraak.Cctv.Service.Application.Queries.GetEngineerDashboard;
using Ashraak.Cctv.Service.Application.Queries.GetEngineerSchedules;
using Ashraak.Cctv.Service.Application.Queries.GetEngineerTodaySchedules;
using Ashraak.Cctv.Service.Application.Queries.GetEngineerVisitDetail;
using Ashraak.Cctv.Service.Application.Queries.GetPortalUpcomingVisits;
using Ashraak.Cctv.Service.Application.Queries.GetPortalVisitDetail;
using Ashraak.Cctv.Service.Application.Queries.GetPortalVisitHistory;
using Ashraak.Cctv.Service.Application.Queries.GetSchedule;
using Ashraak.Cctv.Service.Application.Queries.GetVisit;
using Ashraak.Cctv.Service.Application.Queries.ListPendingApprovals;
using Ashraak.Cctv.Service.Application.Queries.ListSchedules;
using Ashraak.Cctv.Service.Application.Queries.ListVisits;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Cctv.Service.Api.Endpoints;

/// <summary>Service scheduling and visit endpoints (D1-5) under <c>/api/v1/cctv</c>.</summary>
public static class CctvServiceEndpoints
{
    public static IEndpointRouteBuilder MapCctvServiceEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var schedules = routeBuilder.MapGroup("/cctv/schedules")
            .WithTags("CCTV — Service Scheduling")
            .RequireAuthorization();

        schedules.MapGet("/", ListSchedules)
            .WithName("ListCctvSchedules")
            .WithSummary("Paginated schedule list.");

        schedules.MapGet("/{scheduleId:guid}", GetSchedule)
            .WithName("GetCctvSchedule")
            .WithSummary("Get schedule detail.");

        schedules.MapPost("/", CreateAdHocSchedule)
            .WithName("CreateCctvAdHocSchedule")
            .WithSummary("Create ad-hoc schedule within active term.");

        schedules.MapPost("/{scheduleId:guid}/assign", AssignEngineer)
            .WithName("AssignCctvScheduleEngineer")
            .WithSummary("Assign or reassign engineer.");

        schedules.MapPost("/{scheduleId:guid}/reschedule", RescheduleSchedule)
            .WithName("RescheduleCctvSchedule")
            .WithSummary("Reschedule with reason.");

        schedules.MapPost("/{scheduleId:guid}/cancel", CancelSchedule)
            .WithName("CancelCctvSchedule")
            .WithSummary("Cancel schedule.");

        var visits = routeBuilder.MapGroup("/cctv/visits")
            .WithTags("CCTV — Visits")
            .RequireAuthorization();

        visits.MapGet("/", ListVisits)
            .WithName("ListCctvVisits")
            .WithSummary("Paginated visit list.");

        visits.MapGet("/approvals", ListPendingApprovals)
            .WithName("ListCctvPendingVisitApprovals")
            .WithSummary("Pending approval queue.");

        visits.MapGet("/{visitId:guid}", GetVisit)
            .WithName("GetCctvVisit")
            .WithSummary("Get visit detail.");

        visits.MapPost("/{visitId:guid}/start", StartVisit)
            .WithName("StartCctvVisit")
            .WithSummary("Start visit.");

        visits.MapPut("/{visitId:guid}/remarks", UpdateVisitRemarks)
            .WithName("UpdateCctvVisitRemarks")
            .WithSummary("Update visit remarks.");

        visits.MapPost("/{visitId:guid}/photos", LinkVisitPhoto)
            .WithName("LinkCctvVisitPhoto")
            .WithSummary("Link visit photo.");

        visits.MapPost("/{visitId:guid}/selfie", LinkVisitSelfie)
            .WithName("LinkCctvVisitSelfie")
            .WithSummary("Link visit selfie.");

        visits.MapPost("/{visitId:guid}/location", CaptureVisitLocation)
            .WithName("CaptureCctvVisitLocation")
            .WithSummary("Capture GPS location.");

        visits.MapPost("/{visitId:guid}/signature", LinkVisitSignature)
            .WithName("LinkCctvVisitSignature")
            .WithSummary("Link customer signature.");

        visits.MapPost("/{visitId:guid}/attachments", LinkVisitAttachment)
            .WithName("LinkCctvVisitAttachment")
            .WithSummary("Link visit attachment.");

        visits.MapPost("/{visitId:guid}/submit", SubmitVisitReport)
            .WithName("SubmitCctvVisitReport")
            .WithSummary("Submit visit report for approval.");

        visits.MapPost("/{visitId:guid}/approve", ApproveVisit)
            .WithName("ApproveCctvVisit")
            .WithSummary("Approve visit report.");

        visits.MapPost("/{visitId:guid}/return", ReturnVisit)
            .WithName("ReturnCctvVisit")
            .WithSummary("Return visit for rework.");

        var portalVisits = routeBuilder.MapGroup("/cctv/portal/visits")
            .WithTags("CCTV — Customer Portal")
            .RequireAuthorization();

        portalVisits.MapGet("/upcoming", GetPortalUpcomingVisits)
            .WithName("GetCctvPortalUpcomingVisits")
            .WithSummary("Upcoming visits for own sites.");

        portalVisits.MapGet("/history", GetPortalVisitHistory)
            .WithName("GetCctvPortalVisitHistory")
            .WithSummary("Approved service history.");

        portalVisits.MapGet("/{visitId:guid}", GetPortalVisitDetail)
            .WithName("GetCctvPortalVisitDetail")
            .WithSummary("Approved visit detail.");

        var engineerSchedules = routeBuilder.MapGroup("/cctv/engineer/schedules")
            .WithTags("CCTV — Engineer Portal")
            .RequireAuthorization();

        engineerSchedules.MapGet("/", GetEngineerSchedules)
            .WithName("GetCctvEngineerSchedules")
            .WithSummary("Assigned schedules.");

        engineerSchedules.MapGet("/today", GetEngineerTodaySchedules)
            .WithName("GetCctvEngineerTodaySchedules")
            .WithSummary("Today's assigned work.");

        routeBuilder.MapGet("/cctv/engineer/dashboard", GetEngineerDashboard)
            .WithTags("CCTV — Engineer Portal")
            .RequireAuthorization()
            .WithName("GetCctvEngineerDashboard")
            .WithSummary("Engineer My Day dashboard.");

        routeBuilder.MapGet("/cctv/engineer/visits/{visitId:guid}", GetEngineerVisitDetail)
            .WithTags("CCTV — Engineer Portal")
            .RequireAuthorization()
            .WithName("GetCctvEngineerVisitDetail")
            .WithSummary("Assigned visit detail.");

        routeBuilder.MapPost("/cctv/engineer/visits/sync", OfflineVisitSyncBatch)
            .WithTags("CCTV — Engineer Portal")
            .RequireAuthorization()
            .WithName("SyncCctvEngineerVisits")
            .WithSummary("Batch offline visit sync.");

        return routeBuilder;
    }

    private static async Task<IResult> ListSchedules(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] string? status,
        [FromQuery] Guid? engineerId,
        [FromQuery] Guid? siteId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ListSchedulesQuery(
                tenantContext.TenantId,
                currentUser.UserId,
                page < 1 ? 1 : page,
                pageSize < 1 ? 20 : pageSize,
                fromDate,
                toDate,
                status,
                engineerId,
                siteId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetSchedule(
        Guid scheduleId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetScheduleQuery(tenantContext.TenantId, currentUser.UserId, scheduleId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreateAdHocSchedule(
        [FromBody] CreateAdHocScheduleRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateAdHocScheduleCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                request.ContractTermId,
                request.SiteId,
                request.ScheduledDate,
                request.Notes),
            cancellationToken);

        return ToResult(result, value => Results.Created($"/api/v1/cctv/schedules/{value.Id}", value));
    }

    private static async Task<IResult> AssignEngineer(
        Guid scheduleId,
        [FromBody] AssignEngineerRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AssignEngineerCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                scheduleId,
                request.EngineerId,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> RescheduleSchedule(
        Guid scheduleId,
        [FromBody] RescheduleVisitRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RescheduleScheduleCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                scheduleId,
                request.NewScheduledDate,
                request.Reason,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CancelSchedule(
        Guid scheduleId,
        [FromBody] CancelScheduleRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CancelScheduleCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                scheduleId,
                request.Reason,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetPortalUpcomingVisits(
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetPortalUpcomingVisitsQuery(tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetEngineerSchedules(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetEngineerSchedulesQuery(tenantContext.TenantId, currentUser.UserId, fromDate, toDate),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetEngineerTodaySchedules(
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetEngineerTodaySchedulesQuery(tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetEngineerDashboard(
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetEngineerDashboardQuery(tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> ListVisits(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? reportStatus,
        [FromQuery] Guid? engineerId,
        [FromQuery] Guid? siteId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ListVisitsQuery(
                tenantContext.TenantId,
                currentUser.UserId,
                page < 1 ? 1 : page,
                pageSize < 1 ? 20 : pageSize,
                reportStatus,
                engineerId,
                siteId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetVisit(
        Guid visitId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetVisitQuery(tenantContext.TenantId, currentUser.UserId, visitId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> ListPendingApprovals(
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ListPendingApprovalsQuery(tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> StartVisit(
        Guid visitId,
        [FromBody] StartVisitRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new StartVisitCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                visitId,
                request.StartedAt,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> UpdateVisitRemarks(
        Guid visitId,
        [FromBody] UpdateVisitRemarksRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateVisitRemarksCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                visitId,
                request.Remarks,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> LinkVisitPhoto(
        Guid visitId,
        [FromBody] LinkVisitPhotoRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LinkVisitPhotoCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                visitId,
                request.FileId,
                request.Category,
                null,
                request.CapturedAt),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> LinkVisitSelfie(
        Guid visitId,
        [FromBody] LinkVisitSelfieRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LinkVisitSelfieCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                visitId,
                request.FileId,
                request.CapturedAt),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CaptureVisitLocation(
        Guid visitId,
        [FromBody] CaptureVisitLocationRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CaptureVisitLocationCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                visitId,
                request.Latitude,
                request.Longitude,
                request.CapturedAt),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> LinkVisitSignature(
        Guid visitId,
        [FromBody] LinkVisitSignatureRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LinkVisitSignatureCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                visitId,
                request.FileId,
                request.SignerName,
                request.CapturedAt),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> LinkVisitAttachment(
        Guid visitId,
        [FromBody] LinkVisitAttachmentRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LinkVisitAttachmentCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                visitId,
                request.FileId,
                request.AttachmentType,
                request.Title),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> SubmitVisitReport(
        Guid visitId,
        [FromBody] SubmitVisitReportRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new SubmitVisitReportCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                visitId,
                request.RowVersion,
                request.ClientCorrelationId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> ApproveVisit(
        Guid visitId,
        [FromBody] ApproveVisitRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ApproveVisitCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                visitId,
                request.ReviewRemarks),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> ReturnVisit(
        Guid visitId,
        [FromBody] ReturnVisitRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ReturnVisitCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                visitId,
                request.ReturnReason),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetPortalVisitHistory(
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetPortalVisitHistoryQuery(tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetPortalVisitDetail(
        Guid visitId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetPortalVisitDetailQuery(tenantContext.TenantId, currentUser.UserId, visitId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetEngineerVisitDetail(
        Guid visitId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetEngineerVisitDetailQuery(tenantContext.TenantId, currentUser.UserId, visitId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> OfflineVisitSyncBatch(
        [FromBody] OfflineVisitSyncBatchRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new OfflineVisitSyncBatchCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                request.Items),
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
