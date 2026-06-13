# AMC Plans & Contracts

**Status:** D1-4 implemented (Gate 1)  
**Schema:** `cctv_amc`  
**Implementation phase:** B3 / Sprint 1 D1-4

Aarvii CCTV AMC business module on Ashraak Platform V1.

## Capabilities (D1-4)

- **AMC Plans** — catalog with versioned commercial terms (Draft → Published → Superseded)
- **AMC Contracts** — per-site contracts with full term renewal history
- **Lead conversion** — real `IAmcContractProvisioningService` (published plan version required)
- **Customer portal** — active term view and contract documents
- **Renewal requests** — customer-initiated queue for admin

## Projects

| Layer | Project |
|-------|---------|
| Domain | `Ashraak.Cctv.Amc.Domain` |
| Application | `Ashraak.Cctv.Amc.Application` |
| Infrastructure | `Ashraak.Cctv.Amc.Infrastructure` |
| API | `Ashraak.Cctv.Amc.Api` |

## Documentation

- [Architecture](./architecture.md)
- [Registration](./registration.md)
- [API](./api.md)
- [Events](./events.md)
- [Extending](./extending.md)
- [Operations](./operations.md)
