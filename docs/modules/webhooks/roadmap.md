# Webhook platform roadmap

**W0 (current):** Governance, architecture, event catalog, ADR — **no code**.

---

## W0 — Documentation & governance ✓

- [x] `docs/modules/webhooks/` structure
- [x] Platform manifest
- [x] Event catalog standards
- [x] Delivery, retry, security, operations models
- [x] ADR-Webhook-0001
- [x] Index and architecture cross-links

---

## W1 — Webhook foundation ✓

- [x] `Ashraak.Webhooks.*` module (Domain, Application, Infrastructure, Api)
- [x] `webhooks` schema — `webhook_subscriptions`, `webhook_event_definitions`
- [x] Subscription CRUD + secret rotation API
- [x] `IWebhookPublisher` → outbox (`WebhookRequestedEvent`, `WebhookPublishedDomainEvent`)
- [x] Feature flag `webhooks.enabled`
- [x] Permissions `webhooks:read`, `webhooks:manage`
- [x] Outbox processor for `WebhooksDbContext`
- [x] Unit tests (domain, handlers, publisher, tenant isolation)
- [x] ADR-Webhook-0002 (secret storage)

**Manifest:** Foundation, Registry, Subscription API → Done

---

## W2 — Delivery engine ✓

- [x] `WebhookDelivery` aggregate + `webhook_deliveries` table
- [x] `WebhookDispatcher`, `WebhookDeliveryService`, `WebhookDeliveryHostedService`
- [x] HMAC-SHA256 signing (`X-Webhook-Signature`)
- [x] Canonical payload envelope
- [x] Delivery history API (`GET /deliveries`)
- [x] Audit via delivery domain events
- [x] Correlation propagation
- [x] Health check extension (`/health/ready`)
- [x] Unit tests (payload, signing, delivery, tenant filter)
- [x] ADR-Webhook-0003

**Manifest:** Delivery Engine, HMAC, Logs → Done

---

## W3 — Retry + dead letter ✓

- [x] `WebhookFailureClassifier` (transient / permanent / unknown)
- [x] Exponential backoff retry (`WebhookRetryHostedService`)
- [x] Delivery states: `Retrying`, `DeadLettered`
- [x] `webhook_dead_letters` table + `DeadLetterService`
- [x] Manual retry API (`POST /deliveries/{id}/retry`)
- [x] DLQ replay API (`POST /deadletters/{id}/replay`)
- [x] DLQ read APIs (`GET /deadletters`)
- [x] OpenTelemetry metrics (delivery, retry, DLQ)
- [x] Health check: retry engine registration
- [x] Unit tests (classifier, backoff, outcome, retry domain)
- [x] ADR-Webhook-0004

**Manifest:** Retry Engine, DLQ, Recovery Operations, Monitoring → Done

---

## W4 — Webhook Operations Center ✓

- [x] `modules/webhooks/` React admin module
- [x] Operations center health dashboard
- [x] Subscription CRUD + secret rotation UX
- [x] Delivery history + filters + detail
- [x] Dead letter dashboard + replay
- [x] Manual delivery retry
- [x] Permission guards (`webhooks:read`, `webhooks:manage`)
- [x] Correlation ID copy on errors and details
- [x] Vitest unit tests
- [x] `docs/modules/webhooks/admin-ui/`

**Manifest:** Operations Center, Admin UI → Done

---

## W5 — Mobile visibility

- Read-only webhook delivery status in mobile app
- Recent failures list
- Link to web for configuration

**Manifest:** Mobile visibility → Done

---

## Future (post-W5)

- IP allowlisting per subscription
- Mutual TLS
- Ordered delivery per subscription
- OpenAPI webhook subscription APIs in public SDK
- Webhook sandbox / test ping

---

## Success criteria

Webhooks are **platform-complete** when:

- Any module can publish catalog events without webhook-specific code
- Any external product can subscribe with standard signing
- Delivery is async, retried, auditable, and tenant-isolated
- Manifest rows are Done through W5
