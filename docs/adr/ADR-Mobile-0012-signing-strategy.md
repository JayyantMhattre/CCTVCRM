# ADR-Mobile-0012: Mobile signing strategy

## Status

Accepted (M5)

## Decision

- **Android:** Upload keystore via `key.properties` (local) or GitHub secrets (CI). Unsigned debug fallback for PR builds.
- **iOS:** Certificates via Fastlane Match / Xcode; `ExportOptions.plist` not committed.
- **Never** commit keystores, `.p12`, or passwords.

## Consequences

- `android/key.properties.example` documents local setup
- `ios/ExportOptions.plist.example` documents export settings
- Release workflows decode secrets at runtime only

## References

- [android/README.md](../mobile/release/android/README.md)
- [ios/README.md](../mobile/release/ios/README.md)
