# ADR-0007: Outbox Hosted Processors

**Status:** Accepted  
**Date:** 2026-05-31

## Decision

Implement `OutboxProcessorHostedService<TDbContext>` for Auth, Tenant, and Users; serialize domain events on `SaveChanges` via `BaseDbContext` or `OutboxDomainEventSerializer`.

## Rationale

Reliable at-least-once dispatch; removes synchronous `IPublisher` from registration path; preserves modular boundaries via contract-event bridges.

## Tradeoffs

Auth cannot inherit `BaseDbContext` (Identity) — uses explicit serializer override
