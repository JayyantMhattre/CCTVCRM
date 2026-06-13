# Invoice Management

**Status:** D1-7 complete (Option B invoicing)  
**Schema:** `cctv_invoice`  
**Implementation phase:** B6 / D1-7

Option B invoice management: six invoice types with optional AMC term link (required only for `AmcRenewal` / `NewAmc`), draft line editing, PDF generation on `Generate`, and manual lifecycle (`Sent` → `Paid`).

## Projects

| Layer | Project |
|-------|---------|
| Domain | `Ashraak.Cctv.Invoice.Domain` |
| Application | `Ashraak.Cctv.Invoice.Application` |
| Infrastructure | `Ashraak.Cctv.Invoice.Infrastructure` |
| API | `Ashraak.Cctv.Invoice.Api` |

## Documentation

- [Architecture](./architecture.md)
- [Registration](./registration.md)
- [API](./api.md)
- [Events](./events.md)
- [Extending](./extending.md)
- [Operations](./operations.md)

## Feature flag

`CctvFeatureFlags.InvoicesEnabled` (`cctv.invoices.enabled`)

## Related design

- [ERD — Invoice domain](../../project/design/erd-invoice-domain.md)
- [Endpoint catalog §12](../../project/design/endpoint-catalog.md)
