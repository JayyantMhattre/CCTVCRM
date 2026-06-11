# Versioning strategy

## Single source

`FrontEnd.Mobile/version.yaml`:

```yaml
version: 1.0.0
build: 1
```

## Semantic versioning

`MAJOR.MINOR.PATCH` — aligned with platform releases.

| Bump | When |
|------|------|
| MAJOR | Breaking mobile/API contract |
| MINOR | New manifest capability Done |
| PATCH | Fixes only |

## Build number

- Local: `build` in `version.yaml`
- CI: `github.run_number` via `sync-mobile-version.sh`

## Sync

```powershell
./scripts/sync-mobile-version.ps1 -BuildNumberOverride 42
```

Updates `pubspec.yaml` → `1.0.0+42` (Flutter `versionCode` / `CFBundleVersion`).

## Runtime

`AppVersion` in `lib/core/release/app_version.dart` — compile-time `APP_VERSION`, `APP_BUILD_NUMBER`.

## Release notes

```powershell
./scripts/generate-mobile-release-notes.ps1 -FromTag mobile/v1.0.0
```

ADR: [ADR-Mobile-0011](../../../adr/ADR-Mobile-0011-versioning-strategy.md)
