# Crash reporting (M4)

**Path:** `lib/core/crash_reporting/`

## Interface

`CrashReportingService` — vendor-neutral.

| Implementation | When |
|----------------|------|
| `NoOpCrashReporter` | Release default |
| `LoggingCrashReporter` | Debug / `--dart-define=CRASH_REPORTING_LOG=true` |

## Captures

- `FlutterError.onError`
- `PlatformDispatcher.onError`
- Dio errors via `reportDioError()`

## Future providers

Firebase Crashlytics, Sentry — swap via Riverpod provider binding.

ADR: [ADR-Mobile-0008](../adr/ADR-Mobile-0008-crash-reporting.md)
