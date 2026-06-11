# ADR-Mobile-0010: Release strategy

## Status

Accepted (M5)

## Decision

Four **flavors** (dev, qa, uat, prod) on Android productFlavors + iOS schemes, aligned with existing `AppEnvironment` dart-defines. Releases via **GitHub Actions** (`android-release.yml`, `ios-release.yml`) and **Fastlane** lanes.

## Consequences

- CI flavor matrix validates all environments on PR
- Store promotion requires UAT sign-off before prod
- Bootstrap applies `tooling/android/app-build.gradle.kts` after `flutter create`

## References

- [release/README.md](../mobile/release/README.md)
