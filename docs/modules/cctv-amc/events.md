# Events — AMC Plans & Contracts

Domain events raised in `Ashraak.Cctv.Amc.Domain` (audit / future integration).

## Plan events

| Event | Trigger |
|-------|---------|
| `PlanCreatedDomainEvent` | New plan created |
| `PlanVersionPublishedDomainEvent` | Draft version published; prior Published superseded |
| `PlanRetiredDomainEvent` | Plan retired |

## Contract events

| Event | Trigger |
|-------|---------|
| `ContractCreatedDomainEvent` | Contract created (manual or lead conversion) |
| `TermCreatedDomainEvent` | New term added (New or Renewal origin) |
| `TermActivatedDomainEvent` | Draft term activated; prior Active expired |
| `TermExpiredDomainEvent` | Term expired (domain helper) |
| `RenewalRequestedDomainEvent` | Customer renewal request on active term |
| `ContractPdfGeneratedDomainEvent` | Contract PDF document linked |

Cross-module: Lead conversion uses `IAmcContractProvisioningService` (no direct Lead→Amc domain reference).
