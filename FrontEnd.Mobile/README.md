# Ashraak Mobile (Flutter)

Enterprise Flutter client — **M5: release-ready** (flavors, versioning, Fastlane, store CI).

## Prerequisites

- Flutter SDK 3.24+ (stable)
- Android Studio / Xcode for device builds

## First-time setup

If `android/` or `ios/` folders are missing:

```bash
cd FrontEnd.Mobile
flutter create . --org com.ashraak --project-name ashraak_mobile --platforms=android,ios
flutter pub get
```

Or run from repo root:

```powershell
./scripts/bootstrap-mobile-platform.ps1
```

## Run (Dev)

```bash
flutter run --dart-define=ENV=dev
```

## Quality

```bash
flutter analyze
flutter test
```

## Release builds

```bash
./scripts/build-mobile.sh qa appbundle
./scripts/sync-mobile-version.sh    # sync version.yaml -> pubspec
```

Flavors: `dev`, `qa`, `uat`, `prod` — see [docs/mobile/release/](../docs/mobile/release/README.md).

## Documentation

- [docs/mobile/foundation/](../docs/mobile/foundation/README.md)
- [docs/mobile/release/](../docs/mobile/release/README.md)
- [Platform manifest](../docs/mobile/mobile-platform-manifest.md)

## Environments

| `ENV` dart-define | API base |
|-------------------|----------|
| `dev` (default) | `http://10.0.2.2:5000` (Android emulator) |
| `qa` | from `EnvironmentConfig` |
| `uat` | from `EnvironmentConfig` |
| `prod` | from `EnvironmentConfig` |
