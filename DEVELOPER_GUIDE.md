# Ashraak — Developer Guide
## "How to Create a New SaaS Using This Template"

> **Documentation hub:** Canonical, audited docs live under [docs/index.md](docs/index.md).  
> **Governance:** All future features require documentation per [docs/documentation-governance.md](docs/documentation-governance.md).  
> **Frontend:** This template uses **React 19 + Vite 6** (not Angular) — see [docs/frontend/](docs/frontend/).

This guide walks you through every step from cloning the template to running your first custom module in production. Every file path, class name, and code snippet in this document is taken directly from the live codebase — nothing is generic or fabricated.

---

## Table of Contents

1. [Prerequisites](#1-prerequisites)
2. [Clone the Template](#2-clone-the-template)
3. [Rename the Solution](#3-rename-the-solution)
4. [Enable / Disable Modules](#4-enable--disable-modules)
5. [Add a New Module](#5-add-a-new-module)
6. [Configure Multi-Tenancy](#6-configure-multi-tenancy)
7. [Quick-Start Checklist](#7-quick-start-checklist)
8. [Architecture Reference](#8-architecture-reference)
9. [Master Rename Prompt](#9-master-rename-prompt)

---

## 1. Prerequisites

| Tool | Version | Purpose |
|---|---|---|
| .NET SDK | 10.0.103+ | Backend runtime (pinned in `global.json`) |
| Node.js | 20.x+ | Frontend toolchain |
| pnpm | 9.x+ | Frontend monorepo package manager |
| Docker Desktop | 4.x+ | Infrastructure services (postgres, redis, mongo) |
| Git | any | Source control |

Verify your environment:

```bash
dotnet --version          # must be 10.0.103 or higher
node  --version           # must be 20+
pnpm  --version           # must be 9+
docker --version          # must be installed and running
```

---

## 2. Clone the Template

```bash
# Clone into a new folder named after your product
git clone https://github.com/your-org/ashraak.git  MyProduct
cd MyProduct

# Create a fresh git history (detach from the template's history)
Remove-Item -Recurse -Force .git        # PowerShell
git init
git add .
git commit -m "chore: bootstrap from Ashraak template"
```

### Verify the structure

```
MyProduct/
├── BackEnd/
│   ├── Ashraak.slnx                  ← solution file (rename in Step 3)
│   ├── global.json                   ← .NET SDK pin (10.0.103)
│   ├── Directory.Build.props         ← shared MSBuild settings for ALL projects
│   ├── Directory.Packages.props      ← centralised NuGet version catalogue
│   ├── docker-compose.yml            ← all infrastructure services
│   ├── Dockerfile                    ← multi-stage .NET 10 build
│   ├── .env.example                  ← secrets template
│   └── src/
│       ├── Host/Ashraak.Api/         ← composition root (entry point)
│       ├── Shared/                   ← SharedKernel + SharedKernel.Contracts
│       ├── BuildingBlocks/           ← Application, EventBus, Infrastructure base
│       ├── Infrastructure/           ← shared infra (persistence helpers)
│       └── Modules/                  ← Auth, Tenant, Users, Audit, Caching
└── FrontEnd/
    ├── pnpm-workspace.yaml
    ├── package.json
    └── apps/web/                     ← React 19 + Vite 6 SPA
```

### Start the infrastructure and verify the API builds

```bash
cd BackEnd

# 1. Copy secrets template
Copy-Item .env.example .env

# 2. Start all Docker services (postgres, redis, mongodb, rabbitmq, seq)
docker compose up -d

# 3. Verify the solution builds
dotnet build Ashraak.slnx

# 4. Database schema
#    Docker Postgres runs scripts/init-db.sql (schemas: auth, tenant, users).
#    When EF migrations exist for your fork, apply per module, e.g.:
#    dotnet ef database update --project src/Modules/Auth/Ashraak.Auth.Infrastructure --startup-project src/Host/Ashraak.Api

# 5. Start the API
dotnet run --project src/Host/Ashraak.Api

# 6. Open Scalar API reference
start http://localhost:5000/scalar/v1
```

---

## 3. Rename the Solution

All naming in this template follows the prefix `Ashraak`. Rename it to your product name (e.g. **Orion**) before writing any business logic.

### What needs renaming

| Location | What to change |
|---|---|
| `BackEnd/Ashraak.slnx` | Solution file name |
| Every `Ashraak.*.csproj` file name | Project file names |
| Every `namespace Ashraak.*` in `.cs` files | C# namespaces |
| Every `using Ashraak.*` in `.cs` files | C# using directives |
| `Directory.Build.props` → `<RootNamespace>` | Namespace derivation rule |
| `appsettings.json` → `"serviceName"` in OpenTelemetry | Service identity |
| `Program.cs` → `"Ashraak.Api"` service name | Telemetry label |
| `docker-compose.yml` → container names, image names | Docker identifiers |
| `FrontEnd/apps/web/package.json` → `"name": "@ashraak/web"` | npm package name |
| `FrontEnd/package.json` → `"name": "ashraak-frontend"` | Workspace root name |
| `FrontEnd/apps/web/.env.development` → `VITE_APP_NAME` | Browser tab title |
| `FrontEnd/apps/web/src/main.tsx` → bootstrap comment | App name |

### Manual rename steps (PowerShell)

> **Tip:** Use the [Master Rename Prompt](#9-master-rename-prompt) at the end of this guide to do all of this with one AI agent instruction instead.

```powershell
$old = "Ashraak"
$new = "Orion"      # ← your product name

Set-Location "D:\YourPath\MyProduct\BackEnd"

# 1. Rename all .cs files — update namespaces and using directives
Get-ChildItem -Recurse -Filter "*.cs" | ForEach-Object {
    (Get-Content $_.FullName) -replace $old, $new | Set-Content $_.FullName
}

# 2. Rename all .csproj files — update ProjectReference paths inside them
Get-ChildItem -Recurse -Filter "*.csproj" | ForEach-Object {
    (Get-Content $_.FullName) -replace $old, $new | Set-Content $_.FullName
}

# 3. Rename .csproj files on disk
Get-ChildItem -Recurse -Filter "Ashraak.*.csproj" | Rename-Item -NewName {
    $_.Name -replace $old, $new
}

# 4. Rename project folders
Get-ChildItem -Recurse -Directory | Where-Object { $_.Name -like "$old.*" } |
    Sort-Object FullName -Descending |
    ForEach-Object { Rename-Item $_.FullName ($_.FullName -replace $old, $new) }

# 5. Rename solution file and update its content
Rename-Item "Ashraak.slnx" "Orion.slnx"
(Get-Content "Orion.slnx") -replace $old, $new | Set-Content "Orion.slnx"

# 6. Update docker-compose, appsettings, env files
"docker-compose.yml","docker-compose.override.yml","docker-compose.prod.yml",
"appsettings.json","appsettings.Production.json",".env.example",".env" |
    Where-Object { Test-Path $_ } | ForEach-Object {
        (Get-Content $_) -replace $old.ToLower(), $new.ToLower() -replace $old, $new |
        Set-Content $_
    }
```

```powershell
# 7. Update Frontend
Set-Location "..\FrontEnd"
Get-ChildItem -Recurse -Filter "*.ts","*.tsx","*.json","*.yaml","*.md" -Exclude "node_modules" |
    ForEach-Object {
        (Get-Content $_.FullName) -replace $old.ToLower(), $new.ToLower() -replace $old, $new |
        Set-Content $_.FullName
    }
```

### After rename — verify build

```bash
cd BackEnd
dotnet build Orion.slnx     # must succeed with 0 errors
```

---

## 4. Enable / Disable Modules

Modules are registered in exactly **two** files. No other file in the host project needs to change.

### The registration contract

Every module exposes two extension methods:

```
Add{Module}(IServiceCollection, IConfiguration)   → registers DI services
Map{Module}Endpoints(IEndpointRouteBuilder)        → registers HTTP routes
```

### Where modules are toggled

**File: `BackEnd/src/Host/Ashraak.Api/Extensions/ModuleExtensions.cs`**

```csharp
public static IServiceCollection AddModules(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // Layer 0 — must be first (other modules depend on ICacheService)
    services.AddCachingModule(configuration);

    // Layer 1 — identity (no deps on other modules)
    services.AddAuthModule(configuration);

    // Layer 2 — business data (use Auth contracts for permission checks)
    services.AddTenantModule(configuration);
    services.AddUsersModule(configuration);

    // Layer 3 — cross-cutting observers
    services.AddAuditModule(configuration);

    return services;
}

public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder routeBuilder)
{
    routeBuilder.MapAuthEndpoints();
    routeBuilder.MapTenantEndpoints();
    routeBuilder.MapUserEndpoints();
    routeBuilder.MapAuditEndpoints();

    return routeBuilder;
}
```

### Disabling a module (example: disable Audit)

**Step 1 — Comment out service registration in `ModuleExtensions.cs`:**

```csharp
// services.AddAuditModule(configuration);   // disabled
```

**Step 2 — Comment out endpoint registration:**

```csharp
// routeBuilder.MapAuditEndpoints();          // disabled
```

**Step 3 — Remove project references from `Ashraak.Api.csproj`:**

```xml
<!-- Remove or comment out these two lines -->
<!-- <ProjectReference Include="..\..\Modules\Audit\Ashraak.Audit.Infrastructure\..." /> -->
<!-- <ProjectReference Include="..\..\Modules\Audit\Ashraak.Audit.Api\..." /> -->
```

**Step 4 — Remove the middleware call from `Program.cs`:**

```csharp
// app.UseAuditApiCallLogging();   // disabled with Audit module
```

**Step 5 — Remove the MongoDB health check from `Program.cs`** (it depends on `IMongoClient` registered by the Audit module):

```csharp
// .AddMongoDb(...)   // disabled with Audit module
```

The solution will build cleanly. No other file touches the Audit module.

### Module dependency order (do not change)

```
Caching ──► Auth ──► Tenant ──► Users ──► Audit
  (Layer 0)  (L1)     (L2)       (L2)     (L3)
```

- **Caching** must be first — `ICacheService` is resolved by Auth during its own registration.
- **Auth** must precede Tenant/Users — they reference `IAuthPermissionChecker` from `SharedKernel.Contracts`.
- **Audit** must be last — it intercepts all other modules' DbContexts via `IInterceptor`.

---

## 5. Add a New Module

This section adds a fully working **Billing** module as a concrete example. Substitute "Billing" with your module name.

### 5.1 Create the four projects

```bash
cd BackEnd/src/Modules
mkdir Billing
cd Billing

dotnet new classlib -n Orion.Billing.Domain        -f net10.0
dotnet new classlib -n Orion.Billing.Application   -f net10.0
dotnet new classlib -n Orion.Billing.Infrastructure -f net10.0
dotnet new classlib -n Orion.Billing.Api           -f net10.0

# Add to the solution
cd ../../..
dotnet sln Orion.slnx add \
  src/Modules/Billing/Orion.Billing.Domain/Orion.Billing.Domain.csproj \
  src/Modules/Billing/Orion.Billing.Application/Orion.Billing.Application.csproj \
  src/Modules/Billing/Orion.Billing.Infrastructure/Orion.Billing.Infrastructure.csproj \
  src/Modules/Billing/Orion.Billing.Api/Orion.Billing.Api.csproj
```

### 5.2 Domain layer

**`Billing/Orion.Billing.Domain/Plan.cs`** — aggregate root

```csharp
using Orion.SharedKernel.Domain.Primitives;

namespace Orion.Billing.Domain;

/// <summary>Billing plan aggregate — the core domain entity of the Billing module.</summary>
public sealed class Plan : AggregateRoot  // inherits Entity<Guid> + domain events
{
    private Plan() { }   // EF Core constructor

    public string  Name       { get; private set; } = string.Empty;
    public decimal MonthlyUsd { get; private set; }
    public bool    IsActive   { get; private set; }

    // Factory method — always creates valid instances
    public static Plan Create(string name, decimal monthlyUsd)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var plan = new Plan
        {
            Id         = Guid.NewGuid(),
            Name       = name,
            MonthlyUsd = monthlyUsd,
            IsActive   = true,
        };

        // Raise a domain event — other modules observe via MediatR
        plan.RaiseDomainEvent(new PlanCreatedEvent(plan.Id, plan.Name));
        return plan;
    }

    public void Deactivate() => IsActive = false;
}
```

**`Billing/Orion.Billing.Domain/PlanCreatedEvent.cs`** — domain event

```csharp
using Orion.SharedKernel.Domain.Events;

namespace Orion.Billing.Domain;

/// <summary>Published when a new billing plan is created.</summary>
public sealed record PlanCreatedEvent(Guid PlanId, string Name) : DomainEvent;
```

**`Billing/Orion.Billing.Domain/Repositories/IPlanRepository.cs`** — repository contract

```csharp
namespace Orion.Billing.Domain.Repositories;

/// <summary>Persistence contract for Plan aggregates.</summary>
public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(Guid planId, CancellationToken ct = default);
    Task<IReadOnlyList<Plan>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Plan plan, CancellationToken ct = default);
}
```

### 5.3 Application layer — CQRS command

**`Billing/Orion.Billing.Application/Commands/CreatePlan/CreatePlanCommand.cs`**

```csharp
using MediatR;

namespace Orion.Billing.Application.Commands.CreatePlan;

/// <summary>Command to create a new billing plan.</summary>
/// <param name="Name">Human-readable plan name.</param>
/// <param name="MonthlyUsd">Monthly price in USD.</param>
public sealed record CreatePlanCommand(string Name, decimal MonthlyUsd)
    : IRequest<Guid>;
```

**`Billing/Orion.Billing.Application/Commands/CreatePlan/CreatePlanHandler.cs`**

```csharp
using MediatR;
using Orion.Billing.Domain;
using Orion.Billing.Domain.Repositories;
using Orion.SharedKernel.Interfaces;

namespace Orion.Billing.Application.Commands.CreatePlan;

/// <summary>Handles <see cref="CreatePlanCommand"/> — creates and persists a new plan.</summary>
internal sealed class CreatePlanHandler(
    IPlanRepository plans,
    IUnitOfWork     unitOfWork) : IRequestHandler<CreatePlanCommand, Guid>
{
    public async Task<Guid> Handle(CreatePlanCommand request, CancellationToken ct)
    {
        var plan = Plan.Create(request.Name, request.MonthlyUsd);
        await plans.AddAsync(plan, ct);
        await unitOfWork.SaveChangesAsync(ct);   // IUnitOfWork is already registered globally
        return plan.Id;
    }
}
```

**`Billing/Orion.Billing.Application/Commands/CreatePlan/CreatePlanValidator.cs`**

```csharp
using FluentValidation;

namespace Orion.Billing.Application.Commands.CreatePlan;

/// <summary>Validates <see cref="CreatePlanCommand"/> before the handler runs.</summary>
internal sealed class CreatePlanValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Plan name is required.")
            .MaximumLength(100);

        RuleFor(x => x.MonthlyUsd)
            .GreaterThan(0).WithMessage("Monthly price must be positive.");
    }
}
```

### 5.4 Infrastructure layer — EF Core

**`Billing/Orion.Billing.Infrastructure/Persistence/BillingDbContext.cs`**

```csharp
using Microsoft.EntityFrameworkCore;
using Orion.Billing.Domain;

namespace Orion.Billing.Infrastructure.Persistence;

/// <summary>
/// Isolated DbContext for the Billing module.
/// Uses the <c>billing</c> PostgreSQL schema — no shared tables with other modules.
/// </summary>
public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options)
    : DbContext(options)
{
    public DbSet<Plan> Plans { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Apply all IEntityTypeConfiguration<T> classes in this assembly automatically.
        builder.ApplyConfigurationsFromAssembly(typeof(BillingDbContext).Assembly);

        // Default schema — keeps Billing tables isolated in their own PostgreSQL schema.
        builder.HasDefaultSchema("billing");
    }
}
```

**`Billing/Orion.Billing.Infrastructure/Persistence/Configurations/PlanConfiguration.cs`**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orion.Billing.Domain;

namespace Orion.Billing.Infrastructure.Persistence.Configurations;

internal sealed class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("plans");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.MonthlyUsd).HasColumnType("decimal(18,2)");
    }
}
```

**`Billing/Orion.Billing.Infrastructure/Persistence/Repositories/PlanRepository.cs`**

```csharp
using Microsoft.EntityFrameworkCore;
using Orion.Billing.Domain;
using Orion.Billing.Domain.Repositories;
using Orion.Billing.Infrastructure.Persistence;

namespace Orion.Billing.Infrastructure.Persistence.Repositories;

internal sealed class PlanRepository(BillingDbContext db) : IPlanRepository
{
    public Task<Plan?> GetByIdAsync(Guid planId, CancellationToken ct)
        => db.Plans.FirstOrDefaultAsync(p => p.Id == planId, ct);

    public async Task<IReadOnlyList<Plan>> GetAllAsync(CancellationToken ct)
        => await db.Plans.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Plan plan, CancellationToken ct)
        => await db.Plans.AddAsync(plan, ct);
}
```

**`Billing/Orion.Billing.Infrastructure/BillingModule.cs`** — DI registration entry point

```csharp
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orion.Billing.Application.Commands.CreatePlan;
using Orion.Billing.Domain.Repositories;
using Orion.Billing.Infrastructure.Persistence;
using Orion.Billing.Infrastructure.Persistence.Repositories;
using Orion.SharedKernel.Interfaces;

namespace Orion.Billing.Infrastructure;

/// <summary>
/// DI composition root for the Billing module.
/// Called once from <c>ModuleExtensions.AddModules()</c> in the host project.
/// </summary>
public static class BillingModule
{
    public static IServiceCollection AddBillingModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Connection string: prefer "Billing" key, fallback to "DefaultConnection".
        var connectionString =
            configuration.GetConnectionString("Billing")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Billing DB connection string is required.");

        services.AddDbContext<BillingDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                // Store EF migrations history in the billing schema.
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "billing");
                npgsql.EnableRetryOnFailure(3);
            });

            // Attach global interceptors (e.g., Audit interceptor) without a
            // hard compile-time dependency on the Audit module.
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        // IUnitOfWork — wraps SaveChangesAsync, consumed by command handlers.
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BillingDbContext>());

        // Repository
        services.AddScoped<IPlanRepository, PlanRepository>();

        // MediatR — registers all handlers in this assembly automatically.
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CreatePlanCommand).Assembly));

        // FluentValidation — registers all validators in this assembly.
        services.AddValidatorsFromAssembly(typeof(CreatePlanCommand).Assembly);

        return services;
    }
}
```

### 5.5 API layer — Minimal API endpoints

**`Billing/Orion.Billing.Api/Endpoints/BillingEndpoints.cs`**

```csharp
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Orion.Billing.Application.Commands.CreatePlan;

namespace Orion.Billing.Api.Endpoints;

/// <summary>
/// Versioned REST endpoints for the Billing module.
/// The host passes a route group already prefixed with /api/v{version},
/// so all routes here are relative (e.g. /billing/plans → /api/v1/billing/plans).
/// </summary>
public static class BillingEndpoints
{
    public static IEndpointRouteBuilder MapBillingEndpoints(
        this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder
            .MapGroup("/billing")
            .WithTags("Billing")
            .RequireAuthorization();    // all Billing endpoints require a valid JWT

        // POST /api/v1/billing/plans — create a billing plan (Admin only)
        group.MapPost("/plans", async (
            CreatePlanCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var planId = await sender.Send(command, ct);
            return Results.Created($"/api/v1/billing/plans/{planId}", new { planId });
        })
        .WithName("CreatePlan")
        .WithSummary("Create a new billing plan")
        .RequireAuthorization("AdminOnly");

        // GET /api/v1/billing/plans — list all plans
        group.MapGet("/plans", () => Results.Ok("Phase 2: connect to a query handler"))
            .WithName("ListPlans")
            .WithSummary("List all billing plans");

        return routeBuilder;
    }
}
```

### 5.6 Wire the module into the host

**File: `src/Host/Orion.Api/Extensions/ModuleExtensions.cs`**

Add exactly three lines — one per method:

```csharp
// 1. Add using directive at the top
using Orion.Billing.Infrastructure;
using Orion.Billing.Api.Endpoints;

// 2. Inside AddModules() — after UsersModule, before AuditModule:
services.AddBillingModule(configuration);   // ← ADD

// 3. Inside MapModules():
routeBuilder.MapBillingEndpoints();         // ← ADD
```

**File: `src/Host/Orion.Api/Orion.Api.csproj`**

Add project references:

```xml
<ProjectReference Include="..\..\Modules\Billing\Orion.Billing.Infrastructure\Orion.Billing.Infrastructure.csproj" />
<ProjectReference Include="..\..\Modules\Billing\Orion.Billing.Api\Orion.Billing.Api.csproj" />
```

### 5.7 Add connection string and schema

**`appsettings.json`** — add billing connection string (same DB, new schema):

```json
{
  "ConnectionStrings": {
    "Billing": "Host=localhost;Port=5432;Database=ashraak;Username=ashraak;Password=ashraak_dev;Search Path=billing"
  }
}
```

**`BackEnd/scripts/init-db.sql`** — add the billing schema:

```sql
CREATE SCHEMA IF NOT EXISTS billing;
GRANT ALL PRIVILEGES ON SCHEMA billing TO current_user;
COMMENT ON SCHEMA billing IS 'Billing plans and subscriptions for the Billing module.';
```

**`docker-compose.yml`** — add the env var to the API service:

```yaml
ConnectionStrings__Billing: >-
  Host=${POSTGRES_HOST:-postgres};Port=5432;
  Database=${POSTGRES_DB:-ashraak};
  Username=${POSTGRES_USER:-ashraak};
  Password=${POSTGRES_PASSWORD};
  Search Path=billing
```

### 5.8 Create and run the migration

```bash
# Generate migration
dotnet ef migrations add InitialCreate \
  --project src/Modules/Billing/Orion.Billing.Infrastructure \
  --startup-project src/Host/Orion.Api

# Apply migration
dotnet ef database update \
  --project src/Modules/Billing/Orion.Billing.Infrastructure \
  --startup-project src/Host/Orion.Api
```

### 5.9 Add the frontend module

```
FrontEnd/apps/web/src/modules/billing/
├── api.ts          → billingApi.createPlan(), billingApi.listPlans()
├── types.ts        → PlanDto, CreatePlanRequest
└── pages/
    ├── PlanListPage.tsx
    └── CreatePlanPage.tsx
```

Register routes in `src/core/router/index.tsx` and `routeMap.ts`, and add a nav link in `AppLayout.tsx` wrapped in `<RoleGuard roles={['Admin']} inline>`.

### Complete new-module checklist

- [ ] 4 projects created: Domain, Application, Infrastructure, Api
- [ ] Aggregate root extends `AggregateRoot` from SharedKernel
- [ ] Repository interface in Domain; implementation in Infrastructure
- [ ] `BillingModule.cs` with `AddBillingModule()` extension method
- [ ] `BillingDbContext` with `billing` schema and migrations history table
- [ ] `BillingEndpoints.cs` with `MapBillingEndpoints()` extension method
- [ ] Two lines added to `ModuleExtensions.cs` (`AddBillingModule` + `MapBillingEndpoints`)
- [ ] Two `<ProjectReference>` entries added to `Orion.Api.csproj`
- [ ] Connection string in `appsettings.json`
- [ ] Schema created in `init-db.sql`
- [ ] Migration generated and applied
- [ ] Health check added in `Program.cs` (optional but recommended)
- [ ] Frontend module folder created with `api.ts`, `types.ts`, pages
- [ ] Routes and nav link registered in frontend

---

## 6. Configure Multi-Tenancy

The tenant system is built-in. Here is where each piece lives and how to configure it.

### How tenancy works

```
JWT access token contains "tenant_id" claim
         │
         ▼
TenantResolutionMiddleware (Auth.Api/Middleware/)
         │  reads claim or X-Tenant-ID header
         │  validates tenant is Active
         │  populates ITenantContext
         ▼
EF Core global query filter: .HasQueryFilter(e => e.TenantId == tenantContext.TenantId)
         │  applied automatically to all tenant-scoped entities
         ▼
Audit module captures TenantId on every log entry
```

### Provisioning a tenant

```http
POST /api/v1/tenants
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name":        "Acme Corporation",
  "slug":        "acme",
  "plan":        "Pro",
  "ownerUserId": "00000000-0000-0000-0000-000000000001"
}
```

### Making your domain entities tenant-scoped

1. Add `TenantId` property to the entity:

```csharp
public sealed class Invoice : AggregateRoot
{
    public Guid TenantId { get; private set; }   // ← required for multi-tenancy
    // ... other properties
}
```

2. Apply the query filter in EF Core configuration:

```csharp
// In YourDbContext.OnModelCreating or IEntityTypeConfiguration:
builder.HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
```

3. Inject `ITenantContext` into your DbContext:

```csharp
public sealed class BillingDbContext(
    DbContextOptions<BillingDbContext> options,
    ITenantContext tenantContext) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Invoice>()
            .HasQueryFilter(e => e.TenantId == tenantContext.TenantId);
    }
}
```

### Tenant isolation levels

| Level | Mechanism | Where configured |
|---|---|---|
| Data isolation | EF Core `HasQueryFilter` on `TenantId` | Each module's `DbContext` |
| Auth isolation | `tenant_id` claim in JWT; `TenantResolutionMiddleware` blocks mismatched tenants | `Auth.Api/Middleware/TenantResolutionMiddleware.cs` |
| Audit isolation | `TenantId` field on every `AuditEntry` in MongoDB | `Audit.Infrastructure/AuditModule.cs` |
| Cache isolation | `CacheKeyBuilder` prefixes all keys with `{tenantId}:` | `Caching.Abstractions/CacheKeyBuilder.cs` |

### Tenant settings

Tenant-specific configuration (locale, timezone, password policy, MFA, session timeout) is stored in `TenantSettings` entity in the Tenant module:

```csharp
// Tenant/Ashraak.Tenant.Domain/TenantSettings.cs
public sealed class TenantSettings : Entity<Guid>
{
    public string Locale             { get; set; } = "en-US";
    public string Timezone           { get; set; } = "UTC";
    public int    PasswordMinLength  { get; set; } = 8;
    public bool   RequireMfa         { get; set; } = false;
    public int    SessionTimeoutMin  { get; set; } = 60;
}
```

Retrieve via `GET /api/v1/tenants/current` — the response includes the full `TenantDto` with `settings`.

### Adding a tenant-aware setting

1. Add the property to `TenantSettings`.
2. Create a migration: `dotnet ef migrations add AddMyTenantSetting --project src/Modules/Tenant/Ashraak.Tenant.Infrastructure`.
3. Add a PATCH endpoint to the Tenant API to update it.
4. Read the value in your module via `ITenantService` from `SharedKernel.Contracts`.

---

## 7. Quick-Start Checklist

Copy this list into your project tracker when starting a new SaaS on this template.

### Day 1

- [ ] Clone template and create fresh git repo
- [ ] Run rename script (or use Master Rename Prompt below)
- [ ] Verify `dotnet build YourProduct.slnx` passes with 0 errors
- [ ] Start Docker: `docker compose up -d`
- [ ] Verify database schemas exist (init-db.sql or EF migrations)
- [ ] Verify API starts and `/health/live` returns 200
- [ ] Open Scalar UI at `http://localhost:5000/scalar/v1`
- [ ] Copy `.env.example` → `.env` and fill in dev values

### Week 1

- [ ] Decide which template modules to keep / disable
- [ ] Create your first business module (follow Step 5 exactly)
- [ ] Configure SSO (Google/Microsoft) in `.env`
- [ ] Start frontend: `cd FrontEnd && pnpm install && pnpm dev`
- [ ] Create your first frontend module page

### Before going live

- [ ] Fill all `← REQUIRED` fields in `.env.example` with production values
- [ ] Set strong passwords for all services
- [ ] Generate JWT signing key: `openssl rand -base64 32`
- [ ] Run production compose: `docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d`
- [ ] Verify `/health/ready` returns 200 (checks postgres + redis + mongodb)
- [ ] Set up a reverse proxy (nginx/Traefik) with TLS in front of `ashraak-api:8080`

---

## 8. Architecture Reference

### Backend layers

```
Ashraak.Api (Host / Composition Root)
    │
    ├── SharedKernel          → AggregateRoot, Entity, ValueObject, DomainEvent
    │                           ICurrentUser, ITenantContext, IUnitOfWork
    ├── SharedKernel.Contracts → Cross-module interfaces and domain event DTOs
    │                            (IAuditService, ITenantService, IAuthPermissionChecker)
    ├── BuildingBlocks         → Base Application, EventBus, Infrastructure helpers
    │
    └── Modules/
        ├── Auth       → OpenIddict, ASP.NET Core Identity, RBAC/ABAC, SSO
        ├── Tenant     → Tenant entity, settings, isolation, resolver
        ├── Users      → Profile data, preferences (NOT auth)
        ├── Audit      → MongoDB tamper-evident log, EF Core interceptors, middleware
        └── Caching    → ICacheService (memory + Redis), invalidation strategy
```

### Cross-module communication rules

| From | To | Allowed mechanism |
|---|---|---|
| Any module | Another module | `SharedKernel.Contracts` interface |
| Any module | Another module | In-process domain event (MediatR `INotification`) |
| Any module | Another module | ❌ Direct project reference (FORBIDDEN) |
| Any module | Another module | ❌ Shared DbContext (FORBIDDEN) |

### Key files to know

| File | Purpose |
|---|---|
| `src/Host/Ashraak.Api/Program.cs` | Middleware pipeline; auth policies; health checks |
| `src/Host/Ashraak.Api/Extensions/ModuleExtensions.cs` | **The only place to register modules** |
| `src/Host/Ashraak.Api/Extensions/OpenApiExtensions.cs` | OpenAPI metadata and Scalar UI config |
| `Directory.Build.props` | `net10.0`, nullable, `TreatWarningsAsErrors` — applies to ALL projects |
| `Directory.Packages.props` | All NuGet versions in one place — add new packages here first |
| `src/Shared/Ashraak.SharedKernel.Contracts/` | All cross-module contracts live here |

---

## 9. Master Rename Prompt

Use this prompt in a **new AI agent session** (in this editor or any AI assistant) immediately after cloning the template into your new repository. It will rename every occurrence of `Ashraak` to your product name across the entire monorepo — backend namespaces, project files, solution file, docker compose, frontend package names, env files, and documentation.

---

```
You are a senior .NET architect and React developer.
I have just cloned the Ashraak modular SaaS starter kit template into a new repository.
I need you to rename the ENTIRE project from "Ashraak" to "[NEW_NAME]" with NO errors.

Replace [NEW_NAME] with my actual product name before sending this prompt.

═══════════════════════════════════════════════════════════════════
OLD NAME : Ashraak
NEW NAME : [NEW_NAME]
OLD LOWER: ashraak
NEW LOWER: [new_name]    (lowercase version of your product name)
═══════════════════════════════════════════════════════════════════

The repository structure is:

  BackEnd/
    Ashraak.slnx
    BackEnd.slnx
    Directory.Build.props
    Directory.Packages.props
    global.json
    docker-compose.yml
    docker-compose.override.yml
    docker-compose.prod.yml
    Dockerfile
    .env.example
    .env
    appsettings.json
    src/
      Host/Ashraak.Api/
        Ashraak.Api.csproj
        Program.cs
        appsettings.json
        appsettings.Development.json
        appsettings.Production.json
        Extensions/ModuleExtensions.cs
        Extensions/OpenApiExtensions.cs
        Infrastructure/ (CurrentUser.cs, TenantContext.cs, etc.)
        Middleware/
      Shared/
        Ashraak.SharedKernel/Ashraak.SharedKernel.csproj
        Ashraak.SharedKernel.Contracts/Ashraak.SharedKernel.Contracts.csproj
      BuildingBlocks/
        Ashraak.BuildingBlocks.Application/
        Ashraak.BuildingBlocks.EventBus/
        Ashraak.BuildingBlocks.Infrastructure/
      Infrastructure/
        Ashraak.Infrastructure.Shared/
      Modules/
        Auth/   (Domain, Application, Infrastructure, Api .csproj files)
        Tenant/ (Domain, Application, Infrastructure, Api .csproj files)
        Users/  (Domain, Application, Infrastructure, Api .csproj files)
        Audit/  (Domain, Application, Infrastructure, Api .csproj files)
        Caching/(Abstractions, Redis .csproj files)
    tests/
      Ashraak.Integration.Tests/

  FrontEnd/
    package.json
    pnpm-workspace.yaml
    apps/web/
      package.json
      vite.config.ts
      tsconfig.json
      .env.example
      .env.development
      src/
        (all .ts and .tsx files referencing "Ashraak" or "ashraak")
    packages/ui/
      package.json

RENAME TASKS — perform ALL of the following, in this exact order:

──────────────────────────────────────────────────
BACKEND
──────────────────────────────────────────────────

TASK B1 — SOLUTION FILES
  - Rename: Ashraak.slnx → [NEW_NAME].slnx
  - Rename: BackEnd.slnx → BackEnd.slnx (leave this one as-is)
  - Inside [NEW_NAME].slnx: replace all occurrences of "Ashraak" with "[NEW_NAME]"

TASK B2 — PROJECT FILE NAMES (.csproj)
  Rename every file matching Ashraak.*.csproj to [NEW_NAME].*.csproj.
  The full list:
    Ashraak.Api.csproj                    → [NEW_NAME].Api.csproj
    Ashraak.SharedKernel.csproj           → [NEW_NAME].SharedKernel.csproj
    Ashraak.SharedKernel.Contracts.csproj → [NEW_NAME].SharedKernel.Contracts.csproj
    Ashraak.BuildingBlocks.Application.csproj   → [NEW_NAME].BuildingBlocks.Application.csproj
    Ashraak.BuildingBlocks.EventBus.csproj      → [NEW_NAME].BuildingBlocks.EventBus.csproj
    Ashraak.BuildingBlocks.Infrastructure.csproj → [NEW_NAME].BuildingBlocks.Infrastructure.csproj
    Ashraak.Infrastructure.Shared.csproj  → [NEW_NAME].Infrastructure.Shared.csproj
    Ashraak.Auth.Domain.csproj            → [NEW_NAME].Auth.Domain.csproj
    Ashraak.Auth.Application.csproj       → [NEW_NAME].Auth.Application.csproj
    Ashraak.Auth.Infrastructure.csproj    → [NEW_NAME].Auth.Infrastructure.csproj
    Ashraak.Auth.Api.csproj               → [NEW_NAME].Auth.Api.csproj
    Ashraak.Tenant.Domain.csproj          → [NEW_NAME].Tenant.Domain.csproj
    Ashraak.Tenant.Application.csproj     → [NEW_NAME].Tenant.Application.csproj
    Ashraak.Tenant.Infrastructure.csproj  → [NEW_NAME].Tenant.Infrastructure.csproj
    Ashraak.Tenant.Api.csproj             → [NEW_NAME].Tenant.Api.csproj
    Ashraak.Users.Domain.csproj           → [NEW_NAME].Users.Domain.csproj
    Ashraak.Users.Application.csproj      → [NEW_NAME].Users.Application.csproj
    Ashraak.Users.Infrastructure.csproj   → [NEW_NAME].Users.Infrastructure.csproj
    Ashraak.Users.Api.csproj              → [NEW_NAME].Users.Api.csproj
    Ashraak.Audit.Domain.csproj           → [NEW_NAME].Audit.Domain.csproj
    Ashraak.Audit.Application.csproj      → [NEW_NAME].Audit.Application.csproj
    Ashraak.Audit.Infrastructure.csproj   → [NEW_NAME].Audit.Infrastructure.csproj
    Ashraak.Audit.Api.csproj              → [NEW_NAME].Audit.Api.csproj
    Ashraak.Caching.Abstractions.csproj   → [NEW_NAME].Caching.Abstractions.csproj
    Ashraak.Caching.Redis.csproj          → [NEW_NAME].Caching.Redis.csproj
    Ashraak.Integration.Tests.csproj      → [NEW_NAME].Integration.Tests.csproj

TASK B3 — PROJECT FOLDER NAMES
  Rename every folder that starts with "Ashraak." to start with "[NEW_NAME]."
  The folder list mirrors the .csproj names above.
  Process deepest folders first to avoid path invalidation.

TASK B4 — CONTENT OF ALL .csproj FILES
  Inside every renamed .csproj file:
  - Replace all <ProjectReference Include="...Ashraak..." /> paths
    with the new [NEW_NAME] equivalents.
  - Replace any <RootNamespace>Ashraak...</RootNamespace> values.

TASK B5 — ALL .cs SOURCE FILES
  In EVERY .cs file under BackEnd/src/ and BackEnd/tests/:
  - Replace: namespace Ashraak.   → namespace [NEW_NAME].
  - Replace: using Ashraak.       → using [NEW_NAME].
  - Replace: "Ashraak.Api"        → "[NEW_NAME].Api"   (string literals, e.g. telemetry)
  - Replace: typeof(Ashraak.      → typeof([NEW_NAME].
  Do NOT rename standard .NET namespaces (Microsoft.*, System.*, etc.)

TASK B6 — Directory.Build.props
  - Replace the RootNamespace derivation expression:
    OLD: Ashraak.$(MSBuildProjectName.Replace("Ashraak.", "")...)
    NEW: [NEW_NAME].$(MSBuildProjectName.Replace("[NEW_NAME].", "")...)

TASK B7 — appsettings FILES
  In appsettings.json and appsettings.Production.json:
  - Replace "Ashraak.Api" in OpenTelemetry serviceName.
  - Replace "Ashraak Platform Team" in OpenAPI contact.
  - Replace "platform@ashraak.io" with "platform@[new_name].io"
  - Replace connection string database names: "ashraak" → "[new_name]"
    (lowercase — affects all ConnectionStrings values and MongoDB URL)

TASK B8 — docker-compose FILES
  In docker-compose.yml, docker-compose.override.yml, docker-compose.prod.yml:
  - container_name: ashraak-*   → [new_name]-*
  - image: ashraak/*            → [new_name]/*
  - POSTGRES_DB: ashraak        → [new_name]
  - MONGO_INITDB_DATABASE: ashraak_audit → [new_name]_audit
  - service name ashraak-api    → [new_name]-api
  - MongoDB connection string database name in all env vars

TASK B9 — Dockerfile
  - Replace any label or comment referencing "Ashraak" with "[NEW_NAME]"
  - Replace "Ashraak.Api.dll" ENTRYPOINT with "[NEW_NAME].Api.dll"

TASK B10 — ENV FILES
  In .env.example and .env:
  - Replace: ashraak (lowercase) → [new_name]
  - Replace: ashraak_dev → [new_name]_dev
  - Replace: ashraak_audit → [new_name]_audit

TASK B11 — SCRIPTS
  In BackEnd/scripts/init-db.sql:
  - Replace all comments referencing "Ashraak" → "[NEW_NAME]"

TASK B12 — MARKDOWN / DOCUMENTATION FILES
  In all .md files under BackEnd/ and the repo root:
  - Replace all occurrences of "Ashraak" → "[NEW_NAME]"
  - Replace all occurrences of "ashraak" → "[new_name]"

──────────────────────────────────────────────────
FRONTEND
──────────────────────────────────────────────────

TASK F1 — ROOT package.json
  - "name": "ashraak-frontend"  → "[new_name]-frontend"
  - description: replace "Ashraak" with "[NEW_NAME]"
  - scripts: replace --filter @ashraak/ with --filter @[new_name]/

TASK F2 — apps/web/package.json
  - "name": "@ashraak/web"  → "@[new_name]/web"
  - description: replace Ashraak → [NEW_NAME]

TASK F3 — packages/ui/package.json
  - "name": "@ashraak/ui"  → "@[new_name]/ui"

TASK F4 — pnpm-workspace.yaml
  - No rename needed (contains only path globs)

TASK F5 — vite.config.ts
  - Replace any comment or string referencing "Ashraak" or "ashraak"

TASK F6 — ENV FILES (.env.example, .env.development)
  - VITE_APP_NAME=Ashraak (Dev)  → VITE_APP_NAME=[NEW_NAME] (Dev)

TASK F7 — ALL .ts and .tsx SOURCE FILES
  In every file under FrontEnd/apps/web/src/:
  - Replace: "Ashraak"  →  "[NEW_NAME]"   (string literals, comments, labels)
  - Replace: "ashraak"  →  "[new_name]"   (lowercase — API paths, cookie names)
  - Replace: @ashraak/  →  @[new_name]/   (package imports if packages/ui is used)
  Specifically target:
    src/layouts/AuthLayout.tsx     → app brand name in JSX
    src/layouts/AppLayout.tsx      → sidebar brand name in JSX
    src/main.tsx                   → comments
    src/shared/pages/ForbiddenPage.tsx  → "Ashraak Platform" footer
    src/shared/pages/NotFoundPage.tsx   → "Ashraak Platform" footer
    src/core/auth/authStore.ts     → sessionStorage key "ashraak_session"

TASK F8 — FRONTEND DOCUMENTATION
  In FrontEnd/FRONTEND_STARTER.md:
  - Replace all Ashraak → [NEW_NAME], ashraak → [new_name]

──────────────────────────────────────────────────
VERIFICATION
──────────────────────────────────────────────────

After completing ALL tasks above:

1. Run: dotnet build [NEW_NAME].slnx
   Expected: Build succeeded. 0 Error(s).
   If any error references "Ashraak", fix that missed occurrence.

2. Search for any remaining "Ashraak" (case-insensitive) in all .cs, .csproj,
   .json, .yaml, .yml, .ts, .tsx, .md files:
   Report every remaining occurrence with file path and line number.
   Then fix them.

3. Run: dotnet build [NEW_NAME].slnx again.
   Must produce 0 errors before declaring success.

IMPORTANT RULES:
- Do NOT rename standard library namespaces (Microsoft.*, System.*, OpenIddict.*, etc.)
- Do NOT change package names in Directory.Packages.props (NuGet package IDs are not ours)
- Do NOT change image names in docker-compose.yml for third-party images
  (postgres:17-alpine, redis:7-alpine, mongo:7, etc.)
- Do NOT change the BackEnd.slnx filename (it can stay as BackEnd.slnx)
- Preserve all code logic, comments, and documentation — only rename identifiers
- Report every file changed with a brief summary of what was changed
```

---

*Guide version: 1.0 — generated from live Ashraak codebase, May 2026.*
