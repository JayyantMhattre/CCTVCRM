# ADR-Mobile-0001: State management (Riverpod)

## Status

Accepted (M0)

## Context

Flutter requires an explicit state management approach for auth session, async API data, and feature isolation.

## Options evaluated

| Option | Pros | Cons |
|--------|------|------|
| **Riverpod** | Compile-safe providers, testable, no `BuildContext`, codegen | Learning curve for teams from Provider |
| **Bloc/Cubit** | Predictable events, enterprise patterns, clear separation | Boilerplate, verbose for simple async |
| **Provider** | Simple, official legacy | Superseded; less ergonomic at scale |
| **Cubit only** | Lighter than full Bloc | Still more ceremony than Riverpod for async lists |

## Decision

Use **Riverpod** (`flutter_riverpod` + `riverpod_generator` where appropriate).

## Rationale

- Aligns with async REST patterns (login, paginated audit, file upload progress)
- `ProviderContainer` enables unit tests without widget tree
- Recommended modern default for greenfield Flutter (2024–2026)
- Web parity: replaces Zustand + TanStack Query with `Notifier` / `AsyncNotifier`

## Consequences

- `ProviderScope` wraps app in `app/`
- Features use `features/*/providers/`
- Bloc may be used for a single complex flow only with ADR amendment

## References

- [state-management.md](../mobile/state-management.md)
