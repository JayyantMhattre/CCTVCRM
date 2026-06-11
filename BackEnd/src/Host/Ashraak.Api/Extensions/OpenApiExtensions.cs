using Scalar.AspNetCore;

namespace Ashraak.Api.Extensions;

/// <summary>
/// Centralises all OpenAPI document configuration and Scalar UI wiring.
///
/// <para>
/// .NET 10's native <c>Microsoft.AspNetCore.OpenApi</c> (backed by
/// <c>Microsoft.OpenApi</c> v2.0) is used. A single versioned document named
/// <c>"v1"</c> is produced. Adding a <c>"v2"</c> document only requires
/// an additional <c>AddOpenApi("v2", …)</c> call plus a matching <c>MapOpenApi</c> route.
/// </para>
/// <para>
/// NOTE — Microsoft.OpenApi v2.0 changed the security scheme API significantly
/// (interfaces became immutable, type layout changed). Security scheme injection
/// will be implemented in a follow-up once the v2.0 transformer API stabilises.
/// For now, Scalar is configured via its own <c>WithAuthentication</c> API so
/// the Authorize button works without an OpenAPI security scheme declaration.
/// </para>
/// </summary>
internal static class OpenApiExtensions
{
    // ─── Service Registration ─────────────────────────────────────────────────

    /// <summary>
    /// Registers the native .NET 10 OpenAPI document generator for the <c>v1</c>
    /// document with API title, description and contact metadata.
    /// </summary>
    public static IServiceCollection AddOpenApiDocs(this IServiceCollection services)
    {
        services.AddOpenApi("v1", options =>
        {
            // Populate the Info section that Scalar shows in its sidebar header.
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Title       = "Ashraak API";
                document.Info.Version     = "v1";
                document.Info.Description =
                    "Modular SaaS starter platform. " +
                    "Obtain a Bearer JWT from POST /connect/token, then use the " +
                    "Authorize button (top-right in Scalar) to sign all subsequent requests.";

                document.Info.Contact = new()
                {
                    Name  = "Ashraak Platform Team",
                    Email = "platform@ashraak.io"
                };

                return Task.CompletedTask;
            });
        });

        return services;
    }

    // ─── Middleware / Route Registration ──────────────────────────────────────

    /// <summary>
    /// Maps the OpenAPI JSON spec endpoint and the Scalar interactive reference UI.
    /// Only called in non-production environments.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>OpenAPI JSON → <c>/openapi/v1.json</c></item>
    ///   <item>Scalar UI   → <c>/scalar/v1</c></item>
    /// </list>
    /// </remarks>
    /// <param name="app">The built <see cref="WebApplication"/>.</param>
    public static WebApplication MapOpenApiDocs(this WebApplication app)
    {
        // Native .NET 10 spec — {documentName} matches the key passed to AddOpenApi (i.e. "v1").
        app.MapOpenApi("/openapi/{documentName}.json");

        // Scalar replaces Swagger UI with a richer interactive reference.
        app.MapScalarApiReference("/scalar", options =>
        {
            options
                .WithTitle("Ashraak API Reference")
                .WithTheme(ScalarTheme.DeepSpace)
                // Tell Scalar where to resolve the spec JSON for each version.
                .WithOpenApiRoutePattern("/openapi/{documentName}.json")
                // Default code-sample panel (shown when user expands an operation).
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        return app;
    }
}
