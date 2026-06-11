using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Notifications.Api.Endpoints;

public static class NotificationsEndpoints
{
    public static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/notifications")
            .WithTags("Notifications")
            .RequireAuthorization("AdminOnly");

        group.MapGet("/health", () => Results.Ok(new { status = "ok", module = "Notifications" }))
            .WithName("NotificationsHealth")
            .WithSummary("Notifications module health probe.");

        return routeBuilder;
    }
}
