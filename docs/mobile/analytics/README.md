# Analytics (M4)

**Path:** `lib/core/analytics/`

## Interface

`AnalyticsService` — screen and event tracking with user/tenant context.

| Implementation | When |
|----------------|------|
| `NoOpAnalyticsService` | Release default |
| `LoggingAnalyticsService` | Debug |

## Tracking

- Screen: `AppShellPage` calls `trackScreen(location)` on navigation.
- Events: `trackEvent(name, properties: {...})` from features.

## Future providers

Firebase Analytics, Mixpanel, PostHog — provider swap only.

ADR: [ADR-Mobile-0009](../adr/ADR-Mobile-0009-analytics.md)
