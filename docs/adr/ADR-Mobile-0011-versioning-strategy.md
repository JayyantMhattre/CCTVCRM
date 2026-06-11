# ADR-Mobile-0011: Mobile versioning strategy

## Status

Accepted (M5)

## Decision

**Single source:** `FrontEnd.Mobile/version.yaml` synced to `pubspec.yaml` via `scripts/sync-mobile-version.*`. Semantic versioning `MAJOR.MINOR.PATCH` + monotonic **build number** from CI `run_number`.

## Consequences

- Runtime version via `AppVersion` dart-defines
- Release notes script reads git history
- No manual pubspec version edits in PRs (sync script required)

## References

- [versioning/README.md](../mobile/release/versioning/README.md)
