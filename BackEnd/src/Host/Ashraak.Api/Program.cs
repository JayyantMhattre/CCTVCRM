using Asp.Versioning;
using Ashraak.Api.Extensions;
using Ashraak.ApiKeys.Infrastructure.Middleware;
using Ashraak.Api.Infrastructure;
using Ashraak.Api.Middleware;
using Ashraak.Audit.Api.Middleware;
using Ashraak.Auth.Api.Middleware;
using Ashraak.SharedKernel.Interfaces;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

// ─── Bootstrap Logger ─────────────────────────────────────────────────────────
// A minimal Serilog logger is created before the host so that fatal start-up
// errors (configuration, DB connection) are captured and visible.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ─── Serilog ─────────────────────────────────────────────────────────────
    // Full structured logging — reads from appsettings.json Serilog section,
    // enriches every log line with machine name and thread id, and ships to
    // a local Seq instance for development.
    builder.Host.UseSerilog((context, config) =>
        config.ReadFrom.Configuration(context.Configuration)
              .Enrich.FromLogContext()
              .Enrich.WithMachineName()
              .Enrich.WithThreadId()
              .WriteTo.Console()
              .WriteTo.Seq(context.Configuration["Seq:Url"] ?? "http://localhost:5341"));

    // ─── Core Infrastructure ──────────────────────────────────────────────────
    // Cross-cutting host services shared by all modules.
    // Modules resolve these through constructor injection without a direct
    // reference to this project.
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUser, CurrentUser>();           // wraps ClaimsPrincipal
    builder.Services.AddScoped<ITenantContext, TenantContext>();        // resolves JWT tenant claim
    builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>(); // clock abstraction for tests

    // ─── Feature Modules ─────────────────────────────────────────────────────
    // Each module is registered via a single extension method that encapsulates
    // all its services, repositories, DbContext, and domain-event handlers.
    // Order: Caching → Auth → Tenant → Users → Audit  (see ModuleExtensions).
    builder.Services.AddModules(builder.Configuration);
    builder.Services.AddOutboxProcessors(builder.Configuration);
    builder.Services.AddHostPlatformServices(builder.Configuration);
    builder.ValidateAshraakEnvironment();

    // ─── API Versioning ───────────────────────────────────────────────────────
    // URL-segment versioning (/api/v1/...) is the default strategy.
    // Clients may also supply an "api-version" query string or the
    // "x-api-version" header for tooling/testing convenience.
    // AssumeDefaultVersionWhenUnspecified keeps health + protocol endpoints
    // (which have no version segment) working without breaking changes.
    //
    // NOTE — AddApiExplorer is intentionally omitted. It belongs to the MVC
    // API Explorer pipeline. Minimal API versioning is fully handled by the
    // NewApiVersionSet / MapGroup pattern below, and .NET 10 native OpenAPI
    // generates the spec directly from the endpoint metadata.
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion                   = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions                   = true;

        // Accept version from URL segment (primary), query string, or header.
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("x-api-version"));
    });

    // ─── OpenAPI / Scalar ─────────────────────────────────────────────────────
    // Registers the native .NET 10 document generator (v1) with:
    //   • API title, version, description, contact
    //   • JWT Bearer security scheme (lock icon in Scalar UI)
    builder.Services.AddOpenApiDocs();

    // ─── Exception Handling ───────────────────────────────────────────────────
    // RFC 7807 ProblemDetails for all 4xx/5xx responses.
    // GlobalExceptionHandler converts unhandled exceptions to HTTP 500.
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // ─── Output Cache ─────────────────────────────────────────────────────────
    // Redis-backed output cache (see StackExchangeRedis output cache package).
    // Module endpoints can opt in with .CacheOutput() on their routes.
    builder.Services.AddOutputCache(options =>
    {
        options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromSeconds(30)));
    });

    // ─── Authentication & Authorisation ──────────────────────────────────────
    // AddAuthentication() without arguments defers to per-module scheme
    // registration (OpenIddict in Auth module, Google/Microsoft in Auth module).
    // Policies are centrally declared here so all modules can reference them
    // by name without knowing each other's internal role strings.
    builder.Services.AddAuthentication();

    builder.Services.AddAuthorization(options =>
    {
        // Requires "Admin" role — used by Audit endpoints.
        options.AddPolicy("AdminOnly", policy =>
            policy.RequireRole("Admin"));

        // Requires either "Admin" or "Manager" — used by Tenant management.
        options.AddPolicy("TenantAdmin", policy =>
            policy.RequireRole("Admin", "Manager"));
    });

    // ─── Observability ────────────────────────────────────────────────────────
    // OpenTelemetry traces and metrics are exported to the configured OTLP
    // collector (Jaeger / Grafana Tempo in development, production endpoint
    // set via OTEL_EXPORTER_OTLP_ENDPOINT env variable).
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r
            .AddService(
                serviceName:    "Ashraak.Api",
                serviceVersion: "1.0.0"))
        .WithTracing(t => t
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter())
        .WithMetrics(m => m
            .AddAspNetCoreInstrumentation()
            .AddMeter("Ashraak.Webhooks.Delivery")
            .AddOtlpExporter());

    // ─── Health Checks ────────────────────────────────────────────────────────
    // /health/live  — responds once the process is up (liveness probe).
    // /health/ready — responds once all dependencies are reachable (readiness probe).
    builder.Services.AddHealthChecks()
        .AddNpgSql(
            connectionString: builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty,
            name:             "postgres",
            tags:             ["ready", "db"])
        .AddRedis(
            redisConnectionString: builder.Configuration.GetConnectionString("Redis") ?? string.Empty,
            name:                  "redis",
            tags:                  ["ready"])
        // AspNetCore.HealthChecks.MongoDb 9.x removed the connection-string overload.
        // IMongoClient is registered as a singleton by AddAuditModule, so we resolve it here.
        .AddMongoDb(
            clientFactory: sp => sp.GetRequiredService<MongoDB.Driver.IMongoClient>(),
            name:          "mongodb",
            tags:          ["ready"])
        .AddPlatformHealthChecks();

    // ═════════════════════════════════════════════════════════════════════════
    var app = builder.Build();
    // ═════════════════════════════════════════════════════════════════════════

    // ─── OpenAPI / Scalar UI (development only) ───────────────────────────────
    // Kept off in production to prevent leaking internal API surface.
    // Scalar UI → https://localhost:{port}/scalar/v1
    // Raw JSON  → https://localhost:{port}/openapi/v1.json
    if (app.Environment.IsDevelopment())
        app.MapOpenApiDocs();

    // ─── Middleware Pipeline ──────────────────────────────────────────────────
    // ORDER IS CRITICAL — each middleware runs in declaration order for the
    // request and in reverse order for the response.
    //
    //  1. ExceptionHandler      — converts unhandled exceptions to ProblemDetails
    //  2. CorrelationMiddleware — X-Correlation-Id → Serilog + OpenTelemetry baggage
    //  3. SerilogRequestLogging — structured HTTP access log
    //  4. RateLimitingMiddleware — Redis-backed limits for abuse-prone routes
    //  5. Authentication        — validates JWT / cookie and populates ClaimsPrincipal
    //  6. TenantResolution      — resolves TenantId from JWT claim or X-Tenant-ID header
    //  7. Authorization         — evaluates policy/role requirements
    //  8. AuditApiCallLogging   — captures HTTP call metadata into the audit queue
    //  9. OutputCache           — returns cached responses (must come AFTER auth/authz)

    app.UseExceptionHandler();
    app.UseCorrelationMiddleware();
    app.UseSerilogRequestLogging();
    app.UseRateLimitingMiddleware();

    app.UseAuthentication();
    app.UseApiKeyAuthentication();
    app.UseTenantResolution();      // Auth module middleware — see TenantResolutionMiddleware
    app.UseAuthorization();
    app.UseApiKeyUsageTracking();
    app.UseAuditApiCallLogging();   // Audit module middleware — see AuditApiCallMiddleware
    app.UseOutputCache();

    // ─── Endpoint Registration ────────────────────────────────────────────────

    // Protocol endpoints — fixed, non-versioned paths required by OAuth2 / OpenIddict.
    // Must be mapped BEFORE the versioned group so routing resolution order is correct.
    app.MapModuleProtocolEndpoints();

    // Versioned REST API — all module REST endpoints live under /api/v{version}.
    // RouteGroupBuilder implements IEndpointRouteBuilder, so each module mapper
    // receives it transparently and appends its own sub-group (/auth, /tenants …).
    var apiVersionSet = app
        .NewApiVersionSet()
        .HasApiVersion(new ApiVersion(1, 0))
        .ReportApiVersions()
        .Build();

    var v1 = app
        .MapGroup("/api/v{version:apiVersion}")
        .WithApiVersionSet(apiVersionSet)
        .MapToApiVersion(new ApiVersion(1, 0));

    // Register all module endpoint groups under the versioned prefix.
    v1.MapModules();

    // ─── Health Check Endpoints ───────────────────────────────────────────────
    // /health — all checks (structured JSON)
    // /health/live — liveness (process up)
    // /health/ready — readiness (postgres, redis, mongodb, notifications, outbox)
    app.MapPlatformHealthChecks();

    await app.RunAsync();
}
catch (Exception ex)
{
    // Any exception thrown during host start-up is logged and the process exits.
    Log.Fatal(ex, "Ashraak.Api failed to start");
}
finally
{
    // Ensures all buffered Serilog events are flushed before the process exits.
    await Log.CloseAndFlushAsync();
}

/// <summary>Exposes the implicit Program class for integration test WebApplicationFactory.</summary>
public partial class Program;
