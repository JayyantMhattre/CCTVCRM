using Ashraak.Cctv.Customer.Application.Commands.ChangeCustomerStatus;
using Ashraak.Cctv.Customer.Application.Commands.CreateCustomer;
using Ashraak.Cctv.Customer.Application.Commands.UpdateCustomer;
using Ashraak.Cctv.Customer.Application.Commands.UpdateOwnProfile;
using Ashraak.Cctv.Customer.Application.Queries.GetCustomer;
using Ashraak.Cctv.Customer.Application.Queries.GetCustomerSites;
using Ashraak.Cctv.Customer.Application.Queries.GetPortalProfile;
using Ashraak.Cctv.Customer.Application.Queries.ListCustomers;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Cctv.Customer.Api.Endpoints;

/// <summary>Customer management endpoints (D1-2) under <c>/api/v1/cctv</c>.</summary>
public static class CctvCustomerEndpoints
{
    public static IEndpointRouteBuilder MapCctvCustomerEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/cctv/customers")
            .WithTags("CCTV — Customer")
            .RequireAuthorization();

        group.MapGet("/", ListCustomers)
            .WithName("ListCctvCustomers")
            .WithSummary("Paginated customer list.");

        group.MapGet("/{customerId:guid}", GetCustomer)
            .WithName("GetCctvCustomer")
            .WithSummary("Get customer detail.");

        group.MapPost("/", CreateCustomer)
            .WithName("CreateCctvCustomer")
            .WithSummary("Create a manual customer.");

        group.MapPut("/{customerId:guid}", UpdateCustomer)
            .WithName("UpdateCctvCustomer")
            .WithSummary("Update customer fields.");

        group.MapPatch("/{customerId:guid}/status", ChangeCustomerStatus)
            .WithName("ChangeCctvCustomerStatus")
            .WithSummary("Activate or deactivate a customer.");

        group.MapGet("/{customerId:guid}/sites", GetCustomerSites)
            .WithName("GetCctvCustomerSites")
            .WithSummary("List sites for a customer.");

        var portal = routeBuilder.MapGroup("/cctv/portal")
            .WithTags("CCTV — Customer Portal")
            .RequireAuthorization();

        portal.MapGet("/profile", GetPortalProfile)
            .WithName("GetCctvPortalProfile")
            .WithSummary("Get own customer profile (Customer role).");

        portal.MapPatch("/profile", UpdatePortalProfile)
            .WithName("UpdateCctvPortalProfile")
            .WithSummary("Update own profile (name, phone, email only).");

        return routeBuilder;
    }

    private static async Task<IResult> ListCustomers(
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
            new ListCustomersQuery(
                tenantContext.TenantId,
                currentUser.UserId,
                page < 1 ? 1 : page,
                pageSize < 1 ? 20 : pageSize,
                status,
                search),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetCustomer(
        Guid customerId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCustomerQuery(tenantContext.TenantId, currentUser.UserId, customerId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreateCustomer(
        [FromBody] CreateCustomerRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateCustomerCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                request.Name,
                request.Email,
                request.Phone,
                request.BillingAddress,
                request.City),
            cancellationToken);

        return ToResult(result, value => Results.Created($"/api/v1/cctv/customers/{value.Id}", value));
    }

    private static async Task<IResult> UpdateCustomer(
        Guid customerId,
        [FromBody] UpdateCustomerRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateCustomerCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                customerId,
                request.Name,
                request.Email,
                request.Phone,
                request.BillingAddress,
                request.City,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> ChangeCustomerStatus(
        Guid customerId,
        [FromBody] ChangeCustomerStatusRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ChangeCustomerStatusCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                customerId,
                request.Status,
                request.RowVersion),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetCustomerSites(
        Guid customerId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCustomerSitesQuery(tenantContext.TenantId, currentUser.UserId, customerId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> GetPortalProfile(
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetPortalProfileQuery(tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> UpdatePortalProfile(
        [FromBody] UpdateOwnProfileRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateOwnProfileCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                request.Name,
                request.Phone,
                request.Email,
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
