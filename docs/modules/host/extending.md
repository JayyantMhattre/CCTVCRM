# Host — Extending

## Add a new feature module

### 1. Create module projects

Follow existing vertical slice: `Domain`, `Application`, `Infrastructure`, `Api`.

### 2. Add `*Module.cs` in Infrastructure

```csharp
public static class MyModule
{
    public static IServiceCollection AddMyModule(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext, repos, MediatR, etc.
        return services;
    }
}
```

### 3. Add `*Endpoints.cs` in Api

```csharp
public static class MyEndpoints
{
    public static IEndpointRouteBuilder MapMyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/my-resource");
        // Map endpoints...
        return app;
    }
}
```

### 4. Wire in ModuleExtensions.cs

```csharp
// AddModules — respect dependency order
services.AddMyModule(configuration);

// MapModules
routeBuilder.MapMyEndpoints();
```

### 5. Reference in Ashraak.Api.csproj

```xml
<ProjectReference Include="..\..\Modules\MyModule\Ashraak.MyModule.Infrastructure\..." />
<ProjectReference Include="..\..\Modules\MyModule\Ashraak.MyModule.Api\..." />
```

### 6. Add to solution

`BackEnd/Ashraak.slnx`

## Add middleware

Register in `Program.cs` at the correct pipeline position:

| Concern | Typical position |
|---------|------------------|
| Pre-auth | Before `UseAuthentication()` |
| Post-auth tenant | After `UseAuthentication()`, before `UseAuthorization()` |
| Cross-cutting logging | After auth, before endpoints |
| Response caching | Late (after audit) |

Example:

```csharp
app.UseAuthentication();
app.UseMyMiddleware();  // add here if needs authenticated user
app.UseTenantResolution();
app.UseAuthorization();
```

For module middleware, add extension method in module Api project (pattern: `UseAuditApiCallLogging`).

## Add authorization policy

In `Program.cs`:

```csharp
options.AddPolicy("MyPolicy", policy =>
    policy.RequireRole("MyRole"));
```

Apply on endpoints:

```csharp
.RequireAuthorization("MyPolicy")
```

## Add health check

```csharp
builder.Services.AddHealthChecks()
    .AddCheck("my-service", () => HealthCheckResult.Healthy(), tags: ["ready"]);
```

## Add OpenTelemetry instrumentation

Extend OpenTelemetry configuration in `Program.cs` with additional `AddSource()` or instrumentations.

## Protocol vs versioned endpoints

| Type | Register via | Example |
|------|--------------|---------|
| Versioned REST | `MapModules()` on versioned group | `/api/v1/tenants` |
| Fixed protocol URL | `MapModuleProtocolEndpoints()` on root app | `/connect/token` |

OAuth/OpenIddict paths must stay unversioned.

## Frontend considerations

If adding endpoints consumed by React SPA:

1. Update `FrontEnd/apps/web/src/core/api/endpoints.ts`
2. Ensure Vite proxy covers new path prefixes
3. Document versioned vs unversioned paths explicitly

## Dependency on Caching

If the module uses `ICacheService`, register **after** `AddCachingModule` in `AddModules`. Caching must remain Layer 0.

## Architecture tests

Add layer boundary tests in `BackEnd/tests/Ashraak.Architecture.Tests/` for the new module.
