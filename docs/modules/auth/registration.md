# Auth — Registration

Entry point: `BackEnd/src/Modules/Auth/Ashraak.Auth.Infrastructure/AuthModule.cs`

Called from host `ModuleExtensions.AddModules` as Layer 1 (after Caching).

## DbContext

```csharp
services.AddDbContext<AuthDbContext>((sp, options) =>
{
    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.MigrationsHistoryTable("__ef_migrations_history", "auth");
        npgsql.EnableRetryOnFailure(3);
    });
    options.AddInterceptors(sp.GetServices<IInterceptor>());
});
```

Connection: `ConnectionStrings:Auth` → fallback `ConnectionStrings:DefaultConnection`

## Identity

```csharp
services.AddIdentityCore<IdentityUser<Guid>>(options => { /* password rules */ })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AuthDbContext>();
```

## OpenIddict

Full server + core + validation registration — see [architecture.md](./architecture.md#openiddict-configuration).

## External authentication

```csharp
services.AddAuthentication()
    .AddCookie("Auth.External", /* 10 min sliding */)
    .AddGoogle(/* Authentication:Google:* */, callback /api/auth/sso/google/callback)
    .AddMicrosoftAccount(/* Authentication:Microsoft:* */, callback /api/auth/sso/microsoft/callback);
```

## Application services

| Registration | Lifetime |
|--------------|----------|
| `IUnitOfWork` → `AuthDbContext` | Scoped |
| `IAuthUserRepository` → `AuthUserRepository` | Scoped |
| `AuthAuthorizationRepository` | Scoped |
| `IPasswordHasher` → `Argon2PasswordHasher` | Scoped |
| `IAuthPermissionChecker` → `AuthPermissionChecker` | Scoped |
| `IDomainEventPublisher` → `DomainEventPublisher` | Scoped |

## MediatR and validation

```csharp
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));
services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);
```

**Note:** `ValidationBehavior` from BuildingBlocks is not registered — validators may not auto-run.

## Cross-module contract

```csharp
services.AddScoped<IAuthPermissionChecker, AuthPermissionChecker>();
```

Implements `Ashraak.SharedKernel.Contracts.Auth.Interfaces.IAuthPermissionChecker`.

## Endpoint registration

Two mappers in `AuthEndpoints.cs`:

| Method | Called from | Routes |
|--------|-------------|--------|
| `MapAuthEndpoints()` | `MapModules()` | Versioned `/auth/*` |
| `MapAuthProtocolEndpoints()` | `MapModuleProtocolEndpoints()` | Unversioned `/connect/*`, `/api/auth/sso/*` |

## Middleware registration

In host `Program.cs` (not in AuthModule):

```csharp
app.UseAuthentication();
app.UseTenantResolution();  // Auth.Api extension
app.UseAuthorization();
```

## Configuration keys

| Key | Required | Purpose |
|-----|----------|---------|
| `ConnectionStrings:Auth` | No (fallback DefaultConnection) | PostgreSQL auth schema |
| `Authentication:Google:ClientId` | For Google SSO | Empty string if missing |
| `Authentication:Google:ClientSecret` | For Google SSO | |
| `Authentication:Microsoft:ClientId` | For Microsoft SSO | |
| `Authentication:Microsoft:ClientSecret` | For Microsoft SSO | |
| `Auth:SigningKeyBase64` | **Not read by code** | Docker env only (aspirational) |

Indirect: `ConnectionStrings:Redis` (via Caching module for permissions and sessions).

## Project references

- `Ashraak.Caching.Abstractions` (Infrastructure, Api)
- `Ashraak.SharedKernel`, `Ashraak.SharedKernel.Contracts`
- `Ashraak.BuildingBlocks.Application`
- OpenIddict.AspNetCore 5.4.0, OpenIddict.EntityFrameworkCore 5.4.0
