# Environments

**Decision:** [ADR-Mobile-0003](../../adr/ADR-Mobile-0003-environment-configuration.md).

## Environments

| `ENV` (`--dart-define`) | Default API base | Verbose logging |
|-------------------------|------------------|-----------------|
| `dev` (default) | `http://10.0.2.2:5000` | Yes |
| `qa` | `API_BASE_URL` or `https://api-qa.ashraak.example` | Yes |
| `uat` | `API_BASE_URL` or `https://api-uat.ashraak.example` | No |
| `prod` | `API_BASE_URL` or `https://api.ashraak.example` | No |

`10.0.2.2` is the Android emulator loopback to host `localhost:5000`.

## Usage

```bash
flutter run --dart-define=ENV=dev
flutter run --dart-define=ENV=qa --dart-define=API_BASE_URL=https://api-qa.mycompany.com
```

## Resolution

1. `AppEnvironment.fromDartDefine()` reads `ENV`
2. `EnvironmentConfig.forEnvironment()` returns URLs and flags
3. Providers expose config to Dio, logging, and router

## CI / release

- **CI:** `--dart-define=ENV=dev` for debug builds
- **QA/UAT/Prod:** inject `API_BASE_URL` at build time (Fastlane / CI secrets in M4)
- No production URLs in feature code

## iOS simulator

Use `http://127.0.0.1:5000` or host machine IP — override with `API_BASE_URL` when not on Android emulator defaults.
