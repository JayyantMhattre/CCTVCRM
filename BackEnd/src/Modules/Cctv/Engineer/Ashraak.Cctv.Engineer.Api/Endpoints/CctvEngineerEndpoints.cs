using Ashraak.Cctv.Engineer.Application.Commands.ChangeEngineerStatus;
using Ashraak.Cctv.Engineer.Application.Commands.CreateEngineer;
using Ashraak.Cctv.Engineer.Application.Commands.UpdateEngineer;
using Ashraak.Cctv.Engineer.Application.Queries.GetEngineer;
using Ashraak.Cctv.Engineer.Application.Queries.GetEngineerWorkload;
using Ashraak.Cctv.Engineer.Application.Queries.ListEngineers;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Cctv.Engineer.Api.Endpoints;

/// <summary>Engineer management endpoints (D1-8) under <c>/api/v1/cctv</c>.</summary>
public static class CctvEngineerEndpoints
{
    public static IEndpointRouteBuilder MapCctvEngineerEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/cctv/engineers")
            .WithTags("CCTV — Engineer")
            .RequireAuthorization();

        group.MapGet("/", ListEngineers)
            .WithName("ListCctvEngineers")
            .WithSummary("Paginated engineer list.");

        group.MapGet("/{engineerId:guid}", GetEngineer)
            .WithName("GetCctvEngineer")
            .WithSummary("Get engineer detail.");

        group.MapPost("/", CreateEngineer)
            .WithName("CreateCctvEngineer")
            .WithSummary("Create an engineer record.");

        group.MapPut("/{engineerId:guid}", UpdateEngineer)
            .WithName("UpdateCctvEngineer")
            .WithSummary("Update engineer profile.");

        group.MapPatch("/{engineerId:guid}/status", ChangeEngineerStatus)
            .WithName("ChangeCctvEngineerStatus")
            .WithSummary("Activate or deactivate an engineer.");

        group.MapGet("/{engineerId:guid}/workload", GetEngineerWorkload)
            .WithName("GetCctvEngineerWorkload")
            .WithSummary("Active schedule and ticket assignments for an engineer.");

        return routeBuilder;
    }

    private static async Task<IResult> ListEngineers(
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
            new ListEngineersQuery(
                tenantContext.TenantId,
                currentUser.UserId,
                page < 1 ? 1 : page,
                pageSize < 1 ? 20 : pageSize,
                status,
                search),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetEngineer(
        Guid engineerId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetEngineerQuery(tenantContext.TenantId, currentUser.UserId, engineerId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreateEngineer(
        [FromBody] CreateEngineerRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateEngineerCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                request.Name,
                request.Phone,
                request.PlatformUserId),
            cancellationToken);

        return ToResult(result, value => Results.Created($"/api/v1/cctv/engineers/{value.Id}", value));
    }

    private static async Task<IResult> UpdateEngineer(
        Guid engineerId,
        [FromBody] UpdateEngineerRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateEngineerCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                engineerId,
                request.Name,
                request.Phone,
                request.PlatformUserId,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> ChangeEngineerStatus(
        Guid engineerId,
        [FromBody] ChangeEngineerStatusRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ChangeEngineerStatusCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                engineerId,
                request.Status,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetEngineerWorkload(
        Guid engineerId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetEngineerWorkloadQuery(tenantContext.TenantId, currentUser.UserId, engineerId),
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
