# Ticket Management

**Status:** D1-6 implemented  
**Schema:** `cctv_ticket`  
**Implementation phase:** D1-6 / B5 foundation

Aarvii CCTV AMC business module on Ashraak Platform V1.

## Projects

| Layer | Project |
|-------|---------|
| Domain | `Ashraak.Cctv.Ticket.Domain` |
| Application | `Ashraak.Cctv.Ticket.Application` |
| Infrastructure | `Ashraak.Cctv.Ticket.Infrastructure` |
| API | `Ashraak.Cctv.Ticket.Api` |

## Capabilities (D1-6)

- Ticket aggregate with lifecycle (Open → Assigned → InProgress → Resolved → Closed → Reopened)
- Comments, attachments (max 5), engineer assignment history, status history
- Admin, customer portal, and engineer queue endpoints
- Cross-module validation via `ISiteLookupService`, `IAmcContractLookupService`, `IEngineerLookupService`

## Documentation

- [Architecture](./architecture.md)
- [Registration](./registration.md)
- [API](./api.md)
- [Events](./events.md)
- [Extending](./extending.md)
- [Operations](./operations.md)
