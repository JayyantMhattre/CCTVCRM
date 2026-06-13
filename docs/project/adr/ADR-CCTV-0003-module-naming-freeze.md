# ADR-CCTV-0003 — CCTV Module Naming Freeze

**Status:** Accepted  
**Date:** Sprint 0 approval (2026-06-11)  
**Context:** Sprint 0 established eight CCTV business modules. Naming must not drift across B1–B7.

## Decision

Freeze all CCTV naming conventions documented in [cctv-module-naming-freeze.md](../cctv-module-naming-freeze.md):

- C# projects: `Ashraak.Cctv.{Module}.{Layer}`
- PostgreSQL schemas: `cctv_{domain}` (seven schemas)
- API prefix: `/api/v1/cctv/*`
- Docs: `docs/modules/cctv-{slug}/`
- Permissions: `{resource}:{action}` per permission catalog
- Feature flags: `cctv.{area}.enabled`

## Consequences

- Any rename requires an approved change request.
- New modules outside the frozen set are out of V1 scope.
- [cctv-module-map.md](../cctv-module-map.md) is the authoritative module inventory.

## Related

- [ADR-CCTV-0001](./ADR-CCTV-0001-sms-provider-strategy.md)
- [ADR-CCTV-0002](./ADR-CCTV-0002-pdf-generation-strategy.md)
