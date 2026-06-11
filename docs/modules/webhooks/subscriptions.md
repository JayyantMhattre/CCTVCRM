# Webhook subscriptions

Tenant-scoped outbound webhook endpoints. W1 manages metadata and secrets; W2 delivers events.

---

## Aggregate: `WebhookSubscription`

| Field | Description |
|-------|-------------|
| `Id` | Subscription identifier |
| `TenantId` | Owning tenant (query filter enforced) |
| `Name` | Unique per tenant |
| `EndpointUrl` | Subscriber HTTPS URL (validated) |
| `SecretProtected` | Signing secret (Data Protection at rest) |
| `Enabled` | Active flag |
| `CreatedBy` | User who created |
| `CreatedOnUtc` / `UpdatedOnUtc` | Timestamps |

Secrets are returned **once** on create and rotate — never on GET.

---

## Lifecycle domain events (audit)

| Event | When |
|-------|------|
| `WebhookSubscriptionCreatedDomainEvent` | Create |
| `WebhookSubscriptionUpdatedDomainEvent` | Update |
| `WebhookSubscriptionDisabledDomainEvent` | Disable |
| `WebhookSecretRotatedDomainEvent` | Rotate secret |

Captured by platform `DomainEventAuditHandler` and EF change interceptor.

---

## Validation rules

- Name required; unique per tenant
- Endpoint must be valid absolute URI
- HTTPS required when `Webhooks:RequireHttpsEndpoints` is true
- Feature flag `webhooks.enabled` must be on for tenant

---

## Event catalog

Global definitions in `webhook_event_definitions` — seeded at startup. See [event-catalog.md](./event-catalog.md).

Publishing uses `IWebhookPublisher` — enqueues `WebhookRequestedEvent` and `WebhookPublishedDomainEvent` to outbox only.
