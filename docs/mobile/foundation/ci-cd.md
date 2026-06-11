# CI/CD (foundation)

Workflows: `.github/workflows/mobile.yml`, `android-release.yml`, `ios-release.yml`

## Triggers

Pull requests and pushes to `main`/`master` when `FrontEnd.Mobile/` or mobile scripts change.

## Jobs

| Job | Runner | Steps |
|-----|--------|-------|
| `analyze-test` | `ubuntu-latest` | Bootstrap platforms if missing → `pub get` → `analyze` → `test` |
| `build-android` | `ubuntu-latest` | Debug APK (`--dart-define=ENV=dev`) |
| `build-ios` | `macos-latest` | Debug iOS build (`--no-codesign`) |

## Bootstrap

If `android/` or `ios/` are absent (M1 repo may omit platform folders):

```bash
flutter create . --org com.ashraak --project-name ashraak_mobile --platforms=android,ios
```

Local equivalent: `scripts/bootstrap-mobile-platform.ps1`

## Release CI (M5)

- `android-release.yml` — signed/unsigned AAB/APK artifacts
- `ios-release.yml` — iOS/IPA artifacts
- Flavor matrix on PR (`dev`, `qa`, `uat`, `prod`)

See [release/README.md](../release/README.md).

## Future

- OpenAPI drift check in release gate
- Automated Play/App Store upload (Fastlane plugin wiring)

## Local parity

```bash
flutter analyze --fatal-infos
flutter test
```

Match CI before opening a mobile PR.
