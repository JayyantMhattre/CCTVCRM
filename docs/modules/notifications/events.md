# Notifications — Events

Handlers subscribe to **contract events** in `SharedKernel.Contracts`:

| Event | Template | Handler |
|-------|----------|---------|
| `UserCreatedEvent` | `welcome` | `UserCreatedNotificationHandler` |
| `UserInvitedEvent` | `invitation` | `UserInvitedNotificationHandler` |
| `TenantProvisionedEvent` | `welcome` | `TenantProvisionedNotificationHandler` |
| `TenantSuspendedEvent` | `tenant-suspended` | `TenantSuspendedNotificationHandler` |

Events are published after outbox processors run domain-event bridges (e.g. `UserProfileCreatedDomainEvent` → `UserCreatedEvent`).

`UserInvitedEvent` has no publisher in template yet — handler is ready when invite flow is added.
