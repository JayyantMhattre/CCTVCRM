using Ashraak.Cctv.Amc.Application.Commands.ActivateContractTerm;
using Ashraak.Cctv.Amc.Application.Commands.CancelContract;
using Ashraak.Cctv.Amc.Application.Commands.CreateContract;
using Ashraak.Cctv.Amc.Application.Commands.CreateContractTerm;
using Ashraak.Cctv.Amc.Application.Commands.CreatePlan;
using Ashraak.Cctv.Amc.Application.Commands.CreatePlanVersion;
using Ashraak.Cctv.Amc.Application.Commands.LinkContractDocument;
using Ashraak.Cctv.Amc.Application.Commands.PublishPlanVersion;
using Ashraak.Cctv.Amc.Application.Commands.RetirePlan;
using Ashraak.Cctv.Amc.Application.Commands.SubmitRenewalRequest;
using Ashraak.Cctv.Amc.Application.Commands.UpdatePlan;
using Ashraak.Cctv.Amc.Application.Queries.GetContract;
using Ashraak.Cctv.Amc.Application.Queries.GetContractDocuments;
using Ashraak.Cctv.Amc.Application.Queries.GetPlan;
using Ashraak.Cctv.Amc.Application.Queries.GetPlanVersion;
using Ashraak.Cctv.Amc.Application.Queries.GetPortalAmc;
using Ashraak.Cctv.Amc.Application.Queries.GetPortalAmcDocuments;
using Ashraak.Cctv.Amc.Application.Queries.ListContracts;
using Ashraak.Cctv.Amc.Application.Queries.ListPlans;
using Ashraak.Cctv.Amc.Application.Queries.ListRenewalRequests;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Cctv.Amc.Api.Endpoints;

/// <summary>AMC management endpoints (D1-4) under <c>/api/v1/cctv</c>.</summary>
public static class CctvAmcEndpoints
{
    public static IEndpointRouteBuilder MapCctvAmcEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var plans = routeBuilder.MapGroup("/cctv/amc-plans")
            .WithTags("CCTV — AMC Plans")
            .RequireAuthorization();

        plans.MapGet("/", ListPlans).WithName("ListCctvAmcPlans");
        plans.MapGet("/{planId:guid}", GetPlan).WithName("GetCctvAmcPlan");
        plans.MapPost("/", CreatePlan).WithName("CreateCctvAmcPlan");
        plans.MapPut("/{planId:guid}", UpdatePlan).WithName("UpdateCctvAmcPlan");
        plans.MapPatch("/{planId:guid}/status", RetirePlan).WithName("RetireCctvAmcPlan");
        plans.MapPost("/{planId:guid}/versions", CreatePlanVersion).WithName("CreateCctvAmcPlanVersion");
        plans.MapGet("/{planId:guid}/versions/{versionId:guid}", GetPlanVersion).WithName("GetCctvAmcPlanVersion");
        plans.MapPost("/{planId:guid}/versions/{versionId:guid}/publish", PublishPlanVersion).WithName("PublishCctvAmcPlanVersion");

        var contracts = routeBuilder.MapGroup("/cctv/contracts")
            .WithTags("CCTV — AMC Contracts")
            .RequireAuthorization();

        contracts.MapGet("/", ListContracts).WithName("ListCctvAmcContracts");
        contracts.MapGet("/{contractId:guid}", GetContract).WithName("GetCctvAmcContract");
        contracts.MapPost("/", CreateContract).WithName("CreateCctvAmcContract");
        contracts.MapPost("/{contractId:guid}/terms", CreateContractTerm).WithName("CreateCctvAmcContractTerm");
        contracts.MapPost("/{contractId:guid}/terms/{termId:guid}/activate", ActivateContractTerm).WithName("ActivateCctvAmcContractTerm");
        contracts.MapPatch("/{contractId:guid}/status", CancelContract).WithName("CancelCctvAmcContract");
        contracts.MapGet("/{contractId:guid}/documents", GetContractDocuments).WithName("GetCctvAmcContractDocuments");
        contracts.MapPost("/{contractId:guid}/documents", LinkContractDocument).WithName("LinkCctvAmcContractDocument");
        contracts.MapPost("/{contractId:guid}/renewal-request", SubmitRenewalRequest).WithName("SubmitCctvAmcRenewalRequest");

        routeBuilder.MapGet("/cctv/renewal-requests", ListRenewalRequests)
            .WithTags("CCTV — AMC Contracts")
            .RequireAuthorization()
            .WithName("ListCctvAmcRenewalRequests");

        var portal = routeBuilder.MapGroup("/cctv/portal")
            .WithTags("CCTV — Customer Portal")
            .RequireAuthorization();

        portal.MapGet("/amc", GetPortalAmc).WithName("GetCctvPortalAmc");
        portal.MapGet("/amc/documents", GetPortalAmcDocuments).WithName("GetCctvPortalAmcDocuments");

        return routeBuilder;
    }

    private static async Task<IResult> ListPlans(
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
            new ListPlansQuery(tenantContext.TenantId, currentUser.UserId, page < 1 ? 1 : page, pageSize < 1 ? 20 : pageSize, status, search),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetPlan(
        Guid planId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPlanQuery(tenantContext.TenantId, currentUser.UserId, planId), cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreatePlan(
        [FromBody] CreateAmcPlanRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreatePlanCommand(tenantContext.TenantId, currentUser.UserId, request.Code, request.Name, request.Description),
            cancellationToken);
        return ToResult(result, value => Results.Created($"/api/v1/cctv/amc-plans/{value.Id}", value));
    }

    private static async Task<IResult> UpdatePlan(
        Guid planId,
        [FromBody] UpdateAmcPlanRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdatePlanCommand(tenantContext.TenantId, currentUser.UserId, planId, request.Name, request.Description, request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> RetirePlan(
        Guid planId,
        [FromBody] RetireAmcPlanRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RetirePlanCommand(tenantContext.TenantId, currentUser.UserId, planId, request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreatePlanVersion(
        Guid planId,
        [FromBody] CreateAmcPlanVersionRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreatePlanVersionCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                planId,
                request.Price,
                request.VisitFrequency,
                request.IncludedServices,
                request.SlaDescription,
                request.EffectiveFrom),
            cancellationToken);
        return ToResult(result, value => Results.Created($"/api/v1/cctv/amc-plans/{planId}/versions/{value.Id}", value));
    }

    private static async Task<IResult> GetPlanVersion(
        Guid planId,
        Guid versionId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPlanVersionQuery(tenantContext.TenantId, currentUser.UserId, planId, versionId), cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> PublishPlanVersion(
        Guid planId,
        Guid versionId,
        [FromBody] PublishAmcPlanVersionRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new PublishPlanVersionCommand(tenantContext.TenantId, currentUser.UserId, planId, versionId, request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> ListContracts(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? status,
        [FromQuery] Guid? siteId,
        [FromQuery] Guid? customerId,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ListContractsQuery(tenantContext.TenantId, currentUser.UserId, page < 1 ? 1 : page, pageSize < 1 ? 20 : pageSize, status, siteId, customerId, search),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetContract(
        Guid contractId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContractQuery(tenantContext.TenantId, currentUser.UserId, contractId), cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreateContract(
        [FromBody] CreateAmcContractRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateContractCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                request.SiteId,
                request.CustomerId,
                request.PlanVersionId,
                request.StartDate,
                request.EndDate,
                request.Price),
            cancellationToken);
        return ToResult(result, value => Results.Created($"/api/v1/cctv/contracts/{value.Id}", value));
    }

    private static async Task<IResult> CreateContractTerm(
        Guid contractId,
        [FromBody] CreateAmcContractTermRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateContractTermCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                contractId,
                request.PlanVersionId,
                request.StartDate,
                request.EndDate,
                request.Price,
                request.TermType),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> ActivateContractTerm(
        Guid contractId,
        Guid termId,
        [FromBody] ActivateAmcTermRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ActivateContractTermCommand(tenantContext.TenantId, currentUser.UserId, contractId, termId, request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CancelContract(
        Guid contractId,
        [FromBody] CancelAmcContractRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CancelContractCommand(tenantContext.TenantId, currentUser.UserId, contractId, request.RowVersion),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetContractDocuments(
        Guid contractId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContractDocumentsQuery(tenantContext.TenantId, currentUser.UserId, contractId), cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> LinkContractDocument(
        Guid contractId,
        [FromBody] LinkAmcContractDocumentRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LinkContractDocumentCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                contractId,
                request.FileId,
                request.DocumentType,
                request.Title,
                request.TermId),
            cancellationToken);
        return ToResult(result, value => Results.Created($"/api/v1/cctv/contracts/{contractId}/documents/{value.Id}", value));
    }

    private static async Task<IResult> ListRenewalRequests(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ListRenewalRequestsQuery(tenantContext.TenantId, currentUser.UserId, page < 1 ? 1 : page, pageSize < 1 ? 20 : pageSize),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> SubmitRenewalRequest(
        Guid contractId,
        [FromBody] AmcRenewalRequestDto request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new SubmitRenewalRequestCommand(tenantContext.TenantId, currentUser.UserId, contractId, request.Message),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetPortalAmc(
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPortalAmcQuery(tenantContext.TenantId, currentUser.UserId), cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetPortalAmcDocuments(
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPortalAmcDocumentsQuery(tenantContext.TenantId, currentUser.UserId), cancellationToken);
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
