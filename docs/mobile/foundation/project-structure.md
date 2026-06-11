# Project structure

Root: `FrontEnd.Mobile/`

```
FrontEnd.Mobile/
├── lib/
│   ├── main.dart              # Entry → bootstrap
│   ├── app/                   # Composition root
│   │   ├── bootstrap.dart
│   │   ├── app.dart
│   │   ├── providers.dart
│   │   └── shell/             # Placeholder pages only
│   ├── core/                  # Platform infrastructure
│   │   ├── api/
│   │   ├── auth/
│   │   ├── config/            # Points to environment/
│   │   ├── correlation/
│   │   ├── environment/
│   │   ├── feature_flags/
│   │   ├── logging/
│   │   ├── navigation/
│   │   ├── notifications/
│   │   └── storage/
│   ├── shared/                # Reusable UI (no business logic)
│   │   ├── theme/
│   │   └── widgets/
│   └── features/              # Scaffold only in M1
│       ├── auth/
│       ├── tenant/
│       ├── users/
│       ├── files/
│       ├── audit/
│       ├── notifications/
│       ├── sessions/
│       └── settings/
├── test/                      # Mirrors lib/ layout
├── packages/                  # Generated OpenAPI SDK (future)
└── pubspec.yaml
```

## Layer responsibilities

### `app/`

Composition root only. Wires `ProviderScope`, `MaterialApp.router`, environment bootstrap. No feature logic.

### `core/`

| Folder | Responsibility |
|--------|----------------|
| `api/` | `BaseApiClient`, Dio interceptors (auth, correlation, logging, retry, refresh) |
| `auth/` | `TokenPair`, `TokenStorage`, `AuthSessionNotifier` — no login UI |
| `config/` | Documentation entry; runtime config lives in `environment/` |
| `correlation/` | `X-Correlation-Id` generation and header parsing |
| `environment/` | `AppEnvironment`, `EnvironmentConfig` (Dev/QA/UAT/Prod) |
| `feature_flags/` | `FeatureFlagService` interface — backend connectable later |
| `logging/` | `AppLogger` — structured, environment-aware |
| `navigation/` | `GoRouter`, route paths, auth redirect |
| `notifications/` | `PushNotificationService` interface — FCM in M4 |
| `storage/` | `SecureStorage` abstraction over `flutter_secure_storage` |

### `shared/`

Theme (`AppTheme`, `AppColors` — CoreUI-inspired) and generic widgets (`PlaceholderScaffold`). No API calls.

### `features/`

One folder per backend module. M1 contains README placeholders only. Implementation starts M2+.

## Rules

1. Features depend on `core/` and `shared/`; never the reverse.
2. No hand-written DTOs in features — use generated SDK ([sdk-generation.md](../sdk-generation.md)).
3. No hardcoded API URLs outside `EnvironmentConfig`.
