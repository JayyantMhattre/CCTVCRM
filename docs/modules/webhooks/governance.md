# Webhook governance

Extends [documentation-governance.md](../../documentation-governance.md) for the webhook **platform capability**.

---

## Mandatory rules (W0+)

1. **Documentation before implementation** — no endpoints, tables, or dispatch code without updated docs in `docs/modules/webhooks/`.
2. **Manifest must be updated** — [platform-manifest.md](./platform-manifest.md) on every capability change.
3. **ADR required** for architectural changes (transport, signing, tenancy model, sync delivery).
4. **Event catalog is authoritative** — new event types require [event-catalog.md](./event-catalog.md) entry before publishers ship.
5. **Platform-first** — no CRM-, lead-, or product-specific webhook APIs; use generic subscription + event types.
6. **Asynchronous only** — never block business transactions on external HTTP delivery.
7. **Tenant isolation** — subscriptions and deliveries are always tenant-scoped unless documented global exception with ADR.

---

## Phase gates

| Phase | Allowed |
|-------|---------|
| **W0** (current) | Docs, ADR, manifest, event standards |
| **W1** | Registry, subscription schema, outbox bridge (foundation) |
| **W2** | Delivery engine, signing, audit writes |
| **W3** | Retry, dead letter, poison handling |
| **W4** | Admin UI (web) |
| **W5** | Mobile read-only visibility |

See [roadmap.md](./roadmap.md).

---

## PR checklist (webhook work)

- [ ] `platform-manifest.md` updated
- [ ] `event-catalog.md` updated if new event types
- [ ] `security.md` updated if signing/auth model changes
- [ ] ADR linked if architecture decision
- [ ] Cross-links in `architecture/eventing.md` if publisher modules change
- [ ] No synchronous webhook delivery in request path

---

## Cross-platform sync

When webhook capability changes:

1. Update `docs/modules/webhooks/`
2. Update [platform-manifest.md](./platform-manifest.md)
3. Update [architecture/module-map.md](../../architecture/module-map.md) platform row
4. Update web admin docs (W4+) and [mobile-platform-manifest.md](../../mobile/mobile-platform-manifest.md) for mobile visibility (W5+)

---

## Ownership

| Area | Owner (default) |
|------|----------------|
| Platform architecture | Platform team |
| Event catalog | Module owners + platform review |
| Security / signing | Platform + security review |
| Operations runbooks | SRE / platform |
