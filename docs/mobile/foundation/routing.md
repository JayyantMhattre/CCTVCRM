# Routing (foundation)

**Decision:** [ADR-Mobile-0002](../../adr/ADR-Mobile-0002-navigation.md) — GoRouter.

## Route map (M1)

| Path | Type | Page | Purpose |
|------|------|------|---------|
| `/` | Public | `SplashPage` | Initial load; redirect |
| `/unauthorized` | Public | `UnauthorizedPage` | No valid session |
| `/home` | Protected | `HomePlaceholderPage` | Authenticated shell |

Defined in `lib/core/navigation/route_paths.dart` and wired in `app_router.dart`.

## Redirect logic

```mermaid
flowchart TD
    A[Splash /] --> B{Session loaded?}
    B -->|loading| A
    B -->|not authenticated| C[/unauthorized]
    B -->|authenticated| D[/home]
    E[Protected route] -->|not authenticated| C
```

`GoRouter` `refreshListenable` ties to auth session changes so token load/clear triggers re-evaluation.

## Public vs protected

- **Public:** splash, unauthorized — no token required.
- **Protected:** home and future feature routes — redirect to unauthorized when `authSessionProvider` is unauthenticated.

## Future (M2+)

- Nested shell with bottom navigation
- Role-based redirects (admin audit, etc.)
- Deep links for invitations and password reset

Feature routes live under `features/*/routes/` or a central route table — amend ADR if structure changes.
