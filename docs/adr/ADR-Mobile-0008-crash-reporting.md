# ADR-Mobile-0008: Crash reporting architecture

## Status

Accepted (M4)

## Decision

`CrashReportingService` interface with `NoOpCrashReporter` default. Debug uses `LoggingCrashReporter`. Future: Crashlytics/Sentry adapters via Riverpod binding only.

## Consequences

- Global handlers registered in bootstrap
- No vendor SDK in feature code
- Dio errors optionally reported with path/status context

## References

- [crash-reporting/README.md](../mobile/crash-reporting/README.md)
