# Webhook event catalog

Authoritative naming and versioning standards for webhook events. **Catalog entries are contracts** — implementers must not invent ad hoc names in code.

---

## Naming convention

Format:

```
domain.entity.action
```

Rules:

1. **Lowercase** segments separated by `.`
2. **Past tense** action verbs (`created`, `updated`, `deleted`, `suspended`, `sent`, `failed`)
3. **Domain** = bounded context (user, tenant, file, notification, subscription, agent)
4. **Entity** = aggregate or resource name (singular)
5. No product codes in names (`crm`, `lms`) — product context lives in subscription metadata

### Examples

| Event name | Publisher (future) | Description |
|------------|-------------------|-------------|
| `user.created` | Users / Auth | User account provisioned |
| `user.updated` | Users | Profile or status changed |
| `user.invited` | Auth | Invitation issued |
| `user.suspended` | Users | Account suspended |
| `tenant.created` | Tenant | Workspace provisioned |
| `tenant.updated` | Tenant | Metadata changed |
| `tenant.suspended` | Tenant | Workspace suspended |
| `file.uploaded` | Files | File stored for tenant |
| `file.deleted` | Files | File soft-deleted |
| `notification.sent` | Notifications | Outbound notification succeeded |
| `notification.failed` | Notifications | Outbound notification failed |
| `password.changed` | Auth | Credential rotated |
| `subscription.activated` | Future billing | Plan activated |
| `agent.completed` | Future AI | Agent run finished |

Legacy spoken names (`UserCreated`) map to catalog IDs in documentation only — **wire format uses dot notation**.

---

## Versioning

HTTP header (future):

```
Webhook-Event-Version: v1
```

Payload type identifier:

```
v1.user.created
```

| Version | Policy |
|---------|--------|
| **v1** | Initial stable schemas per event |
| **v2+** | Additive fields preferred; breaking changes require new major version |

Rules:

1. Subscribers declare accepted versions per subscription.
2. Registry records schema per `vN.event.name`.
3. Deprecation: minimum 90-day overlap (documented in release notes) before v1 retirement.
4. Same domain event may fan out to **internal handlers** and **webhooks** with identical payload core.

---

## Payload envelope (future standard)

```json
{
  "id": "uuid",
  "type": "v1.user.created",
  "occurredAt": "2026-05-31T12:00:00Z",
  "tenantId": "uuid",
  "correlationId": "hex",
  "data": { }
}
```

- `data` shape defined per event in W1 JSON Schema / OpenAPI components.
- No PII beyond what the subscribing tenant already owns unless documented.

---

## Adding a new event

1. Propose name in PR updating this file.
2. Platform review for naming collision and tenancy.
3. Update [platform-manifest.md](./platform-manifest.md) publisher row.
4. Implement publisher + outbox mapping in module phase (not W0).

See [extending.md](./extending.md).
