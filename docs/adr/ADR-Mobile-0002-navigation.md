# ADR-Mobile-0002: Navigation (GoRouter)

## Status

Accepted (M0)

## Context

Flutter apps need declarative routing with auth guards, deep links, and nested shells (dashboard).

## Options evaluated

| Option | Pros | Cons |
|--------|------|------|
| **GoRouter** | Official Flutter team package, URL-based, deep linking, redirect API | Verbose route tables for large apps |
| **AutoRoute** | Code generation, typed routes | Extra build step, generated code churn |
| **Navigator 1.0/2.0 raw** | No dependency | Error-prone, poor deep link story |

## Decision

Use **GoRouter** for all navigation.

## Rationale

- Maintained by Flutter team; stable for enterprise
- `redirect` callback maps cleanly to auth/MFA guards (same concept as web `AuthGuard`)
- Deep linking and web URL parity for universal links
- AutoRoute adds codegen complexity without mandatory benefit at current scale

## Consequences

- Routes defined in `lib/core/navigation/app_router.dart`
- Auth state listened via Riverpod `ref.listen` in router refresh
- Route table documented in [navigation.md](../mobile/navigation.md)

## References

- [navigation.md](../mobile/navigation.md)
