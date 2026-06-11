# Flutter foundation (M1)

Enterprise platform infrastructure for `FrontEnd.Mobile/` — **no business features**.

## Scope

| In scope (M1) | Out of scope |
|---------------|--------------|
| Layered `lib/` structure | Login / MFA UI |
| Riverpod global providers | Feature modules |
| GoRouter public/protected routes | FCM implementation |
| Dio `BaseApiClient` + interceptors | Hand-written API DTOs |
| Token + secure storage foundation | Store release pipeline |
| Environments (Dev/QA/UAT/Prod) | |
| Correlation IDs + structured logging | |
| Placeholder shell (splash, unauthorized, home) | |
| CI: analyze, test, debug builds | |

## Quick start

```powershell
./scripts/bootstrap-mobile-platform.ps1
cd FrontEnd.Mobile
flutter run --dart-define=ENV=dev
```

## Documentation map

| Document | Topic |
|----------|-------|
| [project-structure.md](./project-structure.md) | `app/`, `core/`, `shared/`, `features/` |
| [state-management.md](./state-management.md) | Riverpod foundation |
| [routing.md](./routing.md) | GoRouter shell |
| [api-client.md](./api-client.md) | Dio, interceptors, retries |
| [auth-foundation.md](./auth-foundation.md) | Tokens, secure storage (no UI) |
| [environments.md](./environments.md) | Dev/QA/UAT/Prod |
| [testing.md](./testing.md) | Unit + widget test layout |
| [ci-cd.md](./ci-cd.md) | GitHub Actions `mobile.yml` |

## ADRs

- [ADR-Mobile-0001](../../adr/ADR-Mobile-0001-state-management.md) — Riverpod
- [ADR-Mobile-0002](../../adr/ADR-Mobile-0002-navigation.md) — GoRouter
- [ADR-Mobile-0003](../../adr/ADR-Mobile-0003-environment-configuration.md) — Environments
- [ADR-Mobile-0004](../../adr/ADR-Mobile-0004-secure-token-storage.md) — Secure storage
- [ADR-Mobile-0005](../../adr/ADR-Mobile-0005-openapi-sdk-generation.md) — OpenAPI SDK

## Traceability

Update [mobile-platform-manifest.md](../mobile-platform-manifest.md) on every capability change.
