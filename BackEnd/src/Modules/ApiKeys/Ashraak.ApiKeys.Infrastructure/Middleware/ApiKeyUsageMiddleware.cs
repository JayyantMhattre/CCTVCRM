using Ashraak.ApiKeys.Domain.Repositories;
using Ashraak.ApiKeys.Infrastructure.Observability;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Ashraak.ApiKeys.Infrastructure.Middleware;

/// <summary>Records API key usage metrics after the request completes.</summary>
public sealed class ApiKeyUsageMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyUsageMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(
        HttpContext context,
        IApiKeyRepository repository,
        IUnitOfWork unitOfWork,
        ApiKeyMetrics metrics)
    {
        await _next(context);

        if (!context.Items.TryGetValue("ApiKeyId", out var idObj) || idObj is not Guid apiKeyId)
            return;

        var tenantId = Guid.TryParse(context.User.FindFirst("tenant_id")?.Value, out var tid) ? tid : Guid.Empty;
        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
            ?? context.TraceIdentifier;
        var success = context.Response.StatusCode < 400;

        var apiKey = tenantId != Guid.Empty
            ? await repository.GetByIdAsync(tenantId, apiKeyId, context.RequestAborted)
            : null;

        if (apiKey is not null)
        {
            apiKey.RecordUsage(correlationId, success);
            await unitOfWork.SaveChangesAsync(context.RequestAborted);
        }

        if (tenantId != Guid.Empty)
            metrics.RecordRequest(tenantId, success);
    }
}

public static class ApiKeyUsageMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyUsageTracking(this IApplicationBuilder app) =>
        app.UseMiddleware<ApiKeyUsageMiddleware>();
}
