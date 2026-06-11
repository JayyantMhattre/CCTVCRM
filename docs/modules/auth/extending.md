# Auth — Extending

## Add a new authenticated endpoint

1. Add command/query in `Ashraak.Auth.Application`
2. Map in `AuthEndpoints.MapAuthEndpoints()`:

```csharp
group.MapPost("/my-action", MyHandler)
    .RequireAuthorization();
```

3. Choose versioned (`MapAuthEndpoints`) vs protocol (`MapAuthProtocolEndpoints`) group.

## Add a new grant type to /connect/token

Extend `IssueToken` in `AuthEndpoints.cs`:

- Parse additional `grant_type` values
- Implement client credentials or refresh token validation
- Use OpenIddict server APIs for token creation

Current implementation supports **password grant only**.

## Enable persistent signing keys

Replace ephemeral keys in `AuthModule.cs`:

```csharp
// Remove:
options.AddEphemeralEncryptionKey().AddEphemeralSigningKey();

// Add (example):
var signingKey = new SymmetricSecurityKey(
    Convert.FromBase64String(configuration["Auth:SigningKeyBase64"]!));
options.AddSigningKey(signingKey);
```

Wire `Auth:SigningKeyBase64` from Docker env (`JWT_SIGNING_KEY_BASE64`) — already injected in compose but unused.

**Required for production** — ephemeral keys invalidate all tokens on restart.

## Publish additional contract events

After state changes in handlers:

```csharp
await _publisher.Publish(new UserLoggedInEvent(tenantId, userId), cancellationToken);
```

Add handlers in other modules as needed. Audit auto-captures.

## Add RBAC role or permission

1. Extend authorization tables via EF configuration in `Persistence/Authorization/`
2. Seed or admin API for role assignment (not exposed yet)
3. `AuthPermissionChecker` picks up changes after cache TTL (10 min) or explicit invalidation via `ICacheInvalidationService` (not wired by callers yet)

## Implement ITokenService

Contract exists at `SharedKernel.Contracts/Auth/Interfaces/ITokenService.cs` with no implementation.

Add service in Infrastructure, register in `AuthModule.cs`, use for token revocation/introspection workflows.

## SSO account linking

Extend `CompleteSso` in `AuthEndpoints.cs`:

1. Read external claims from `AuthenticateResult`
2. Find or create `AuthUser` by email + tenant
3. Issue local token or link external login
4. Publish `UserRegisteredEvent` or `UserLoggedInEvent` as appropriate

## Add FluentValidation execution

Register BuildingBlocks `ValidationBehavior` in host or Auth module MediatR config so `RegisterUserCommandValidator` runs automatically.

## Tenant middleware bypass paths

Update `TenantResolutionMiddleware` bypass list if adding new anonymous auth routes. Keep versioned vs unversioned paths consistent (register is at `/api/v1/auth/register`, not `/api/auth/register`).

## Module boundaries

- Do not reference `Ashraak.Users.*` from Auth — use `IUserService` contract if needed
- User profile data belongs in Users module; Auth owns credentials only
