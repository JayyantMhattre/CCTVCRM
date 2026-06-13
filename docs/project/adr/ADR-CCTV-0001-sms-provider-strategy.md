# ADR-CCTV-0001 — SMS Provider Strategy (Sprint 0)

**Status:** Accepted (stub phase)  
**Date:** Sprint 0  
**Context:** Freeze §17 requires Email + SMS. Platform Notifications module is email-only.

## Decision

1. Introduce `ISmsProvider` in `Ashraak.Cctv.Integration.Application` (CCTV scope — **no Auth/Notifications core changes**).
2. Sprint 0 ships `StubSmsProvider` (logs only).
3. Production provider selection deferred to **NR-01** review — candidates: MSG91, Twilio, AWS SNS.
4. Email remains primary channel via platform Notifications until SMS provider is wired.

## Consequences

- OTP and alert SMS flows can be developed against the interface in B1+.
- Dev/staging operates without SMS gateway credentials.
- ADR update required when provider is chosen.
