# Events — Lead Management

**Phase:** B1

Domain events raised by the Lead aggregate (dispatched via platform outbox):

| Event | When | Consumers |
|-------|------|-----------|
| `LeadCreatedDomainEvent` | Website inquiry or admin create | Notifications (future), Audit |
| `LeadStatusChangedDomainEvent` | Pipeline transition | Audit |
| `LeadLostDomainEvent` | Marked Lost | Audit |
| `LeadConvertedDomainEvent` | Successful conversion | Notifications (future), Audit |

Cross-module contract events (SharedKernel.Contracts) will align in B2+ when downstream modules ship.
