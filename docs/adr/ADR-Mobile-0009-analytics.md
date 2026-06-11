# ADR-Mobile-0009: Analytics architecture

## Status

Accepted (M4)

## Decision

`AnalyticsService` interface with `NoOpAnalyticsService` default. Screen tracking in shell; event API for features. Future: Firebase/Mixpanel/PostHog via provider swap.

## Consequences

- User/tenant context set from JWT after auth
- No analytics SDK imports in features
- Debug logging sink for development verification

## References

- [analytics/README.md](../mobile/analytics/README.md)
