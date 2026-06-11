# Mobile release engineering (M5)

Store-ready, repeatable release pipeline for `FrontEnd.Mobile/`.

## Quick links

| Topic | Document |
|-------|----------|
| Flavors | [flavors/README.md](./flavors/README.md) |
| Versioning | [versioning/README.md](./versioning/README.md) |
| Android signing | [android/README.md](./android/README.md) |
| iOS signing | [ios/README.md](./ios/README.md) |
| Fastlane | [fastlane/README.md](./fastlane/README.md) |
| Store assets | [store-readiness.md](./store-readiness.md) |
| Process | [release-process.md](./release-process.md) |
| Checklist | [deployment-checklist.md](./deployment-checklist.md) |
| Rollback | [rollback-strategy.md](./rollback-strategy.md) |
| Incidents | [incident-response.md](./incident-response.md) |

## One-command builds

```bash
# From repo root
./scripts/build-mobile.sh qa appbundle
./scripts/sync-mobile-version.sh 42   # CI build number
```

```powershell
./scripts/build-mobile.ps1 -Flavor prod -Target appbundle -BuildNumber 42
```

## CI workflows

| Workflow | Purpose |
|----------|---------|
| `mobile.yml` | PR: analyze, test, flavor smoke builds |
| `android-release.yml` | Release APK/AAB artifacts |
| `ios-release.yml` | Release iOS/IPA artifacts |

## ADRs

- [ADR-Mobile-0010](../../adr/ADR-Mobile-0010-release-strategy.md)
- [ADR-Mobile-0011](../../adr/ADR-Mobile-0011-versioning-strategy.md)
- [ADR-Mobile-0012](../../adr/ADR-Mobile-0012-signing-strategy.md)
