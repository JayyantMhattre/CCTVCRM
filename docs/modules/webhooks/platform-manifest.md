# Webhook platform manifest

**Single source of truth** for webhook capability status, ownership, and dependencies.

**Rule:** Every webhook phase must update this file.

Last updated: W5 (mobile webhook visibility implemented)

---

## Legend

| Status | Meaning |
|--------|---------|
| Planned | Documented; not implemented |
| In Progress | Partial implementation |
| Done | Implemented and documented |
| N/A | Not applicable |

---

## Platform capabilities

| Capability | Status | Owner | Dependencies | Notes |
|------------|--------|-------|--------------|-------|
| **Webhook governance** | Done | Platform | Documentation governance | W0 |
| **Event catalog standards** | Done | Platform | ADR-Webhook-0001 | W0 |
| **Webhook Foundation** | Done | Platform | W1 module | Registry + subscriptions + outbox publish |
| **Webhook Registry** | Done | Platform | `webhook_event_definitions` | Seeded catalog |
| **Webhook Subscription API** | Done | Platform | Auth, Tenant | `/api/v1/webhooks/subscriptions` |
| **Outbox bridge (publish)** | Done | Platform | `IWebhookPublisher` | No HTTP dispatch |
| **HMAC signing** | Done | Platform | ADR-Webhook-0003 | `X-Webhook-Signature` HMAC-SHA256 |
| **Webhook Delivery Engine** | Done | Platform | Outbox, Registry | First-attempt HTTP dispatch W2 |
| **Retry Engine** | Done | Platform | Delivery | Exponential backoff W3 |
| **Dead Letter Queue** | Done | Platform | Retry Engine | `webhook_dead_letters` W3 |
| **Recovery Operations** | Done | Platform | DLQ + APIs | Manual retry/replay W3 |
| **Webhook Audit** | Done | Platform | Domain events + Audit module | Subscriptions + delivery outcomes |
| **Webhook Logs (persisted)** | Done | Platform | `webhook_deliveries` | Read API `/deliveries` |
| **Webhook Monitoring** | Done | Platform | OpenTelemetry | Delivery/retry/DLQ metrics W3 |
| **Webhook Operations Center** | Done | Web | W1–W3 APIs | Admin console W4 |
| **Admin UI (web)** | Done | Web | Operations Center | W4 |
| **Mobile visibility** | Done | Mobile | Logs read API | W5 read-only dashboard |
| **IP allowlisting** | Planned | Platform | Security ADR | Future |
| **Mutual TLS** | Planned | Platform | Security ADR | Future |

---

## Module publisher readiness

| Source module | Example events | Webhook-ready | Notes |
|---------------|----------------|---------------|-------|
| Auth | `user.created`, `user.invited`, `auth.password.changed` | Catalog seeded | Wire `IWebhookPublisher` per module |
| Users | `user.updated` | Catalog seeded | |
| Tenant | `tenant.created`, `tenant.suspended` | Catalog seeded | |
| Files | `file.uploaded`, `file.deleted` | Catalog seeded | |
| Notifications | `notification.sent`, `notification.failed` | Catalog seeded | |
| Audit | N/A (observer) | N/A | Consumes; does not publish webhooks |
| Future (CRM/LMS/ERP) | `subscription.activated`, `agent.completed` | Planned | Add to catalog when modules exist |

---

## Client coverage

| Client | Registry | Logs | Secrets | Retry history | Status |
|--------|----------|------|---------|---------------|--------|
| **Backend** | Done | Done | Done (API) | Done | W3 retry + DLQ + replay |
| **Web** | Done | Done | Done | Done | W4 operations center |
| **Mobile** | N/A | Done (read) | N/A | Done (read) | W5 operational visibility |

---

## Update checklist (per PR)

- [ ] Capability row added or status changed
- [ ] `event-catalog.md` aligned if new events
- [ ] `roadmap.md` phase notes if scope shifts
- [ ] ADR if architectural decision
