# Events — Invoice Management

Domain events raised by the `Invoice` aggregate (schema `cctv_invoice`). Integration handlers deferred to notification phase.

| Domain event | Integration name | When |
|--------------|-------------------|------|
| `InvoiceCreatedDomainEvent` | `invoice.created` | Draft created |
| `InvoiceGeneratedDomainEvent` | `invoice.generated` | PDF generated, status → Generated |
| `InvoiceSentDomainEvent` | `invoice.sent` | Status → Sent |
| `InvoicePaidDomainEvent` | `invoice.paid` | Manual paid |
| `InvoiceCancelledDomainEvent` | `invoice.cancelled` | Cancelled with reason |

See [event-catalog §9](../../project/design/event-catalog.md) for payload fields and notification mapping.

## Status history

Every transition appends an `InvoiceStatusHistory` row (business data, not platform audit).
