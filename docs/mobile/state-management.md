# Mobile state management

**Decision:** [Riverpod](../adr/ADR-Mobile-0001-state-management.md) (with `flutter_riverpod` + code generation where helpful).

---

## Layers

| State type | Mechanism | Example |
|------------|-----------|---------|
| **Session / auth** | `AsyncNotifier` + secure storage | `authSessionProvider` |
| **Feature lists** | `AsyncNotifier` + repository | `auditLogListProvider` |
| **Form/UI ephemeral** | `Notifier` or local `StatefulWidget` | Login form |
| **Global config** | `Provider` (immutable) | `apiConfigProvider` |

---

## Conventions (M1+)

1. **Providers live next to feature** — `features/auth/providers/`, not global god-file.
2. **Repositories** abstract API calls — providers depend on repositories, not raw Dio.
3. **Generated SDK** is the only DTO source — map to view models in repository if needed.
4. **No `setState` for business state** — only for animation/ephemeral UI.
5. **Test with `ProviderContainer`** — override repositories in unit tests.

---

## Comparison (summary)

| Option | Verdict |
|--------|---------|
| **Riverpod** | **Selected** — compile-safe, testable, scales |
| Bloc/Cubit | Strong but verbose; defer unless team standard changes |
| Provider | Legacy; superseded by Riverpod |

Full analysis: [ADR-Mobile-0001](../adr/ADR-Mobile-0001-state-management.md).

---

## Web parity

| Web | Mobile |
|-----|--------|
| Zustand `authStore` | `authSessionProvider` |
| TanStack Query | `AsyncNotifier` + refresh |
| React context | `ProviderScope` at root |
