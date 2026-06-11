# ADR-0008: Host platform hardening

## Status

Accepted

## Context

The Ashraak host needed operational hardening without redesigning the modular monolith: rate limiting, correlation tracing, expanded health checks, startup configuration validation, and a feature-flag foundation.

## Decision

1. **Rate limiting** — Host middleware using existing `ICacheService` and `CacheKeyBuilder.ForRateLimit`; config section `RateLimiting:Routes`; returns 429 + `Retry-After`.
2. **Correlation** — Host middleware `X-Correlation-Id` → Serilog `LogContext` + OpenTelemetry baggage; runs immediately after exception handler.
3. **Health** — Extend existing `AddHealthChecks` with Notifications + Outbox checks; expose `/health`, `/health/live`, `/health/ready` with UI JSON writer.
4. **Environment validation** — `ValidateAshraakEnvironment()` fail-fast before `Build()`; production requires Seq URL and signing key.
5. **Feature flags** — `IFeatureFlagService` contract in SharedKernel; config-backed host implementation only.

## Consequences

- Middleware pipeline order updated (documented in host architecture).
- Production deployments must supply signing key and Seq URL or fail startup.
- No new NuGet rate-limit or feature-flag frameworks.

## Alternatives considered

- AspNetCore.RateLimiting package — rejected to stay lightweight and reuse Redis cache.
- Full LaunchDarkly integration — deferred; foundation only.
