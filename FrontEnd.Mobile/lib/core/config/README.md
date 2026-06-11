# core/config

Runtime configuration entry points.

M1 maps environment resolution to `../environment/`:

- `AppEnvironment` — Dev, QA, UAT, Prod (`--dart-define=ENV=`)
- `EnvironmentConfig` — API base URL, logging flags

No hardcoded production URLs outside `EnvironmentConfig.forEnvironment`.
