using Ashraak.Cctv.Amc.Api.Endpoints;
using Ashraak.Cctv.Customer.Api.Endpoints;
using Ashraak.Cctv.Engineer.Api.Endpoints;
using Ashraak.Cctv.Invoice.Api.Endpoints;
using Ashraak.Cctv.Lead.Api.Endpoints;
using Ashraak.Cctv.Reporting.Api.Endpoints;
using Ashraak.Cctv.Service.Api.Endpoints;
using Ashraak.Cctv.Ticket.Api.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Cctv.Api.Endpoints;

/// <summary>Aggregates all CCTV REST endpoints under <c>/api/v1/cctv</c>.</summary>
public static class CctvEndpoints
{
    private static readonly string[] ModuleNames =
    [
        "Lead", "Customer", "Amc", "Service", "Ticket", "Engineer", "Invoice", "Reporting"
    ];

    public static IEndpointRouteBuilder MapCctvEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/cctv")
            .WithTags("CCTV");

        group.MapGet("/health", GetHealth)
            .WithName("CctvHealth")
            .WithSummary("CCTV module health check.")
            .AllowAnonymous()
            .Produces<CctvHealthResponse>(StatusCodes.Status200OK);

        routeBuilder.MapCctvLeadEndpoints();
        routeBuilder.MapCctvCustomerEndpoints();
        routeBuilder.MapCctvSiteEndpoints();
        routeBuilder.MapCctvAmcEndpoints();
        routeBuilder.MapCctvServiceEndpoints();
        routeBuilder.MapCctvTicketEndpoints();
        routeBuilder.MapCctvEngineerEndpoints();
        routeBuilder.MapCctvInvoiceEndpoints();
        routeBuilder.MapCctvReportingEndpoints();

        return routeBuilder;
    }

    private static IResult GetHealth()
    {
        return Results.Ok(new CctvHealthResponse(
            Status: "healthy",
            Phase: "D1-12",
            Modules: ModuleNames));
    }
}

/// <summary>Response payload for <c>GET /api/v1/cctv/health</c>.</summary>
public sealed record CctvHealthResponse(
    string Status,
    string Phase,
    IReadOnlyList<string> Modules);
