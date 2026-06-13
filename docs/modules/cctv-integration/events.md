# Events — CCTV Integration

CCTV notification and PDF handlers subscribe to domain events via MediatR after outbox dispatch.

## Notification handlers (D1-13c)

Registered in `Ashraak.Cctv.Integration.Infrastructure` — see [notification-mapping](../../project/design/notification-mapping.md).

| Domain event | Handler |
|--------------|---------|
| `LeadCreatedDomainEvent` | `LeadCreatedNotificationHandler` |
| `LeadConvertedDomainEvent` | `LeadConvertedNotificationHandler` |
| `RenewalRequestedDomainEvent` | `RenewalRequestedNotificationHandler` |
| `VisitScheduledDomainEvent` | `VisitScheduledNotificationHandler` |
| `EngineerAssignedDomainEvent` | `EngineerAssignedNotificationHandler` |
| `VisitCompletedDomainEvent` | `VisitCompletedNotificationHandler` |
| `VisitReportApprovedDomainEvent` | `VisitApprovedNotificationHandler` |
| `TicketCreatedDomainEvent` | `TicketCreatedNotificationHandler` |
| `TicketAssignedDomainEvent` | `TicketAssignedNotificationHandler` |
| `TicketClosedDomainEvent` | `TicketClosedNotificationHandler` |
| `InvoiceGeneratedDomainEvent` | `InvoiceGeneratedNotificationHandler` |

**Scheduled (non-event):** `CctvAmcExpiryReminderHostedService` — AMC expiry 30 days before term end.

## PDF handlers (D1-13d)

| Domain event | Handler |
|--------------|---------|
| `TermActivatedDomainEvent` | `TermActivatedContractPdfHandler` |
| `VisitReportApprovedDomainEvent` | `VisitReportApprovedPdfHandler` |

Invoice PDF is generated synchronously in `GenerateInvoiceCommandHandler`.

## Outbox

CCTV module DbContexts register `OutboxProcessorHostedService<T>` in `CctvIntegrationModule`.
