# ADR-0010: Central package management and CI build gate

## Status

Accepted

## Context

Restore failed with NU1010 (missing central versions). CI had docs validation only, no compile gate.

## Decision

1. All NuGet versions remain in `BackEnd/Directory.Packages.props`.
2. `scripts/package-audit.ps1` runs in CI before restore.
3. New workflow `.github/workflows/ci.yml`: package audit → restore → build → test → docs validation.
4. `global.json` uses `rollForward: latestMinor` for developer SDK flexibility.
5. OpenTelemetry packages versioned as a single aligned set (`1.12.0`).

## Consequences

- PRs cannot merge with NU1010 gaps.
- Host requires explicit `OpenTelemetry.Api` for baggage propagation.
- `NU1510` suppressed globally; prefer removing redundant extension packages on `net10.0` where possible.
