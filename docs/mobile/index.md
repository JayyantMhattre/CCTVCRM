# Mobile platform (Flutter)

Ashraak mobile is a **first-class platform citizen** — same backend APIs, same contracts, traceable via the [platform manifest](./mobile-platform-manifest.md).

**Phase M5 (current):** Release engineering — flavors, signing, Fastlane, store readiness, release CI.

---

## Official stack

| Item | Choice |
|------|--------|
| Framework | **Flutter** (single codebase) |
| Targets | Android, iOS |
| Rejected (M0) | Native Kotlin / Swift — see [ADR-0012](../adr/ADR-0012-flutter-mobile-platform.md) |

App root: `FrontEnd.Mobile/` — see [foundation/](./foundation/README.md).

---

## Start here

| I want to… | Go to |
|------------|-------|
| See platform coverage | [Mobile platform manifest](./mobile-platform-manifest.md) |
| Understand architecture | [architecture.md](./architecture.md) |
| Follow mobile rules | [mobile-governance.md](./mobile-governance.md) |
| Integrate APIs | [api-integration.md](./api-integration.md) |
| Generate SDKs | [sdk-generation.md](./sdk-generation.md) |
| Plan releases | [release/](./release/README.md) |
| See delivery phases | [roadmap.md](./roadmap.md) |
| M1 implementation detail | [foundation/](./foundation/README.md) |
| M3 module docs | [modules/](./modules/files/README.md) |

---

## Documentation map

| Document | Purpose |
|----------|---------|
| [architecture.md](./architecture.md) | Layering, `lib/` structure |
| [module-map.md](./module-map.md) | Feature ↔ backend alignment |
| [navigation.md](./navigation.md) | Routing strategy (GoRouter) |
| [state-management.md](./state-management.md) | Riverpod strategy |
| [api-integration.md](./api-integration.md) | Shared REST/OAuth APIs |
| [sdk-generation.md](./sdk-generation.md) | OpenAPI → Dart SDK |
| [security.md](./security.md) | JWT, MFA, tenant, sessions |
| [offline-strategy.md](./offline-strategy.md) | Online-first, future sync |
| [push-notifications.md](./push-notifications.md) | FCM/APNS abstraction |
| [testing-strategy.md](./testing-strategy.md) | Unit, widget, integration |
| [release-process.md](./release-process.md) | Stores, versioning, CI |
| [coding-standards.md](./coding-standards.md) | Dart/Flutter conventions |

---

## Mobile ADRs

| ADR | Topic |
|-----|-------|
| [ADR-0012](../adr/ADR-0012-flutter-mobile-platform.md) | Flutter as official mobile stack |
| [ADR-Mobile-0001](../adr/ADR-Mobile-0001-state-management.md) | Riverpod |
| [ADR-Mobile-0002](../adr/ADR-Mobile-0002-navigation.md) | GoRouter |
| [ADR-Mobile-0003](../adr/ADR-Mobile-0003-environment-configuration.md) | Environment configuration |
| [ADR-Mobile-0004](../adr/ADR-Mobile-0004-secure-token-storage.md) | Secure token storage |
| [ADR-Mobile-0005](../adr/ADR-Mobile-0005-openapi-sdk-generation.md) | OpenAPI SDK generation |
| [ADR-Mobile-0006](../adr/ADR-Mobile-0006-push-notifications.md) | Push notifications |
| [ADR-Mobile-0007](../adr/ADR-Mobile-0007-offline-cache.md) | Offline cache |
| [ADR-Mobile-0008](../adr/ADR-Mobile-0008-crash-reporting.md) | Crash reporting |
| [ADR-Mobile-0009](../adr/ADR-Mobile-0009-analytics.md) | Analytics |
| [ADR-Mobile-0010](../adr/ADR-Mobile-0010-release-strategy.md) | Release strategy |
| [ADR-Mobile-0011](../adr/ADR-Mobile-0011-versioning-strategy.md) | Versioning |
| [ADR-Mobile-0012](../adr/ADR-Mobile-0012-signing-strategy.md) | Signing |

---

## Cross-platform traceability

Every capability must be updatable in three places:

1. Backend module docs (`docs/modules/`)
2. Web docs (`docs/frontend/`)
3. Mobile manifest + module docs (`docs/mobile/`)

See [mobile-governance.md](./mobile-governance.md).
