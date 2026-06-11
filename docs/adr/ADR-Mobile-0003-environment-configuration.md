# ADR-Mobile-0003: Environment configuration

## Status

Accepted (M1)

## Context

Mobile must target Dev, QA, UAT, and Prod without hardcoded URLs in feature code. Builds are flavor-like via compile-time defines rather than separate native product flavors in M1.

## Options evaluated

| Option | Pros | Cons |
|--------|------|------|
| **`--dart-define=ENV` + `EnvironmentConfig`** | Simple CI, matches web env pattern | Requires rebuild per environment |
| **Flutter flavors + native config** | Store-grade separation | Heavy setup for M1 foundation |
| **Runtime remote config only** | Flexible URLs | Fails offline; security review for prod |

## Decision

Use **`AppEnvironment` enum** resolved from `--dart-define=ENV` with **`EnvironmentConfig.forEnvironment()`** supplying `apiBaseUrl`, `tokenUrl`, and logging flags. Non-dev URLs overridable via `--dart-define=API_BASE_URL`.

## Consequences

- All HTTP clients read `environmentConfigProvider`
- CI uses `ENV=dev`
- Store releases (M4) may add flavors; amend ADR if native flavors replace defines

## References

- [environments.md](../mobile/foundation/environments.md)
- Web: [environment.md](../frontend/environment.md)
