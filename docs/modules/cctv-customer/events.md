# Events — Customer / Site / Asset

Domain events raised by aggregates in schema `cctv_customer`. Published via outbox when platform integration is wired.

## Customer (D1-2)

| Event | Trigger | Payload |
|-------|---------|---------|
| `CustomerCreatedDomainEvent` | `CreateManual`, `CreateFromLead` | CustomerId, CustomerNumber, Name, Email, SourceLeadId? |
| `CustomerUpdatedDomainEvent` | `UpdateDetails`, re-activate, `LinkPortalUser`, `UpdateOwnProfile` | CustomerId, CustomerNumber, UpdatedBy |
| `CustomerDeactivatedDomainEvent` | `ChangeStatus` → Inactive | CustomerId, CustomerNumber, DeactivatedBy |

## Site (D1-3)

| Event | Trigger | Payload |
|-------|---------|---------|
| `SiteCreatedDomainEvent` | `CreateManual`, `CreateFromLead` | SiteId, SiteNumber, CustomerId, Name |
| `SiteUpdatedDomainEvent` | `UpdateDetails`, re-activate, document link/remove | SiteId, SiteNumber, UpdatedBy |
| `SiteDeactivatedDomainEvent` | `ChangeStatus` → Inactive | SiteId, SiteNumber, DeactivatedBy |
| `SiteContactChangedDomainEvent` | `ReplaceContacts` | SiteId, SiteNumber, ContactCount, ChangedBy |
| `SiteAssetSummaryUpdatedDomainEvent` | `UpsertAssetSummary`, `UpdateAssetSummary` | SiteId, SiteNumber, UpdatedBy |
