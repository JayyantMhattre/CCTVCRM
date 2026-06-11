# Build flavors

Four environments: **Dev**, **QA**, **UAT**, **Prod**.

## Dart configuration

| Flavor | `--dart-define` | API config |
|--------|-----------------|------------|
| dev | `ENV=dev` | `EnvironmentConfig` emulator host |
| qa | `ENV=qa` | `API_BASE_URL` or QA default |
| uat | `ENV=uat` | `API_BASE_URL` or UAT default |
| prod | `ENV=prod` | `API_BASE_URL` or prod default |

Also pass `FLAVOR=<name>` for native alignment.

## Per-flavor services (`FlavorConfig`)

| Flavor | FCM | Analytics | Crash reporting | App ID suffix |
|--------|-----|-----------|-----------------|---------------|
| dev | Off | Off | Off | `.dev` |
| qa | On | On | On | `.qa` |
| uat | On | On | On | `.uat` |
| prod | On | On | On | (none) |

Code: `lib/core/release/flavor_config.dart`

## Android

Product flavors in `tooling/android/app-build.gradle.kts` — applied via `scripts/apply-mobile-release-config.ps1`.

```bash
flutter build appbundle --release --flavor qa --dart-define=ENV=qa
```

## iOS

Use matching Xcode schemes (`dev`, `qa`, `uat`, `prod`) with bundle ID suffixes. Bootstrap with `flutter create`, then configure schemes per [ios/README.md](../ios/README.md).
