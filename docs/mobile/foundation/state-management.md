# State management (foundation)

**Decision:** [ADR-Mobile-0001](../../adr/ADR-Mobile-0001-state-management.md) — Riverpod.

## M1 scope

Global infrastructure providers only. No feature-level `AsyncNotifier` implementations yet.

## Composition

```
main.dart
  └── bootstrap()
        └── ProviderScope
              └── AshraakApp (ConsumerWidget)
```

## Global providers (`lib/app/providers.dart`)

| Provider | Purpose |
|----------|---------|
| `appEnvironmentProvider` | Resolved `AppEnvironment` from `--dart-define=ENV` |
| `environmentConfigProvider` | API URLs, logging flags |
| `secureStorageProvider` | `FlutterSecureStorage` implementation |
| `tokenStorageProvider` | Read/write `TokenPair` |
| `authSessionProvider` | `AuthSessionNotifier` — authenticated state |
| `appLoggerProvider` | Structured logger |
| `dioProvider` / `baseApiClientProvider` | HTTP stack |

## Auth session

`AuthSessionNotifier` loads tokens from secure storage on startup. GoRouter `redirect` reads `authSessionProvider` to route:

- Unauthenticated → `/unauthorized`
- Authenticated → `/home`

No login form in M1; tokens are set programmatically (M2 login flow).

## Testing

Use `ProviderContainer` with overrides:

```dart
final container = ProviderContainer(overrides: [
  secureStorageProvider.overrideWithValue(mockStorage),
]);
```

## Future (M2+)

Feature folders add `features/{module}/providers/` with scoped notifiers. Prefer `AsyncNotifier` for REST lists and forms.
