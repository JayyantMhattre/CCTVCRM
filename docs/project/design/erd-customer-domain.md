# ERD — Customer Domain

**Schema:** `cctv_customer` · **Modules:** Customer (3), Site (4), Asset (5) Management
**Source of truth:** [requirements-freeze-v1.md §5–§7](../requirements-freeze-v1.md) · Rules: BR-STRUCT-01..05

---

## ER diagram

```mermaid
erDiagram
    Customer ||--o{ Site : "owns (BR-STRUCT-02)"
    Site ||--o{ SiteContact : "max 3 (BR-STRUCT-03)"
    Site ||--o| SiteAssetSummary : "1:1 summary counts"
    Site ||--o{ SiteDocument : "documents"

    Customer {
        uuid id PK
        string customer_number UK "CU-YYYY-NNNN"
        string name
        string email
        string phone
        string billing_address
        string city
        string status "Active | Inactive"
        uuid portal_user_id "nullable - logical ref auth user"
        uuid source_lead_id "nullable - logical ref cctv_lead"
        timestamptz created_at
        uuid created_by
        timestamptz updated_at "nullable"
        uuid updated_by "nullable"
        boolean is_deleted
        bytea row_version
    }

    Site {
        uuid id PK
        uuid customer_id FK "one site - one customer (BR-STRUCT-01)"
        string site_number UK "ST-YYYY-NNNN"
        string name
        string address
        string city
        string status "Active | Inactive"
        timestamptz created_at
        uuid created_by
        timestamptz updated_at "nullable"
        uuid updated_by "nullable"
        boolean is_deleted
        bytea row_version
    }

    SiteContact {
        uuid id PK
        uuid site_id FK
        smallint contact_slot "1..3 - enforces max 3"
        string name
        string designation "nullable"
        string phone
        string email "nullable"
        boolean is_primary
        timestamptz created_at
        uuid created_by
        timestamptz updated_at "nullable"
        uuid updated_by "nullable"
    }

    SiteDocument {
        uuid id PK
        uuid site_id FK
        uuid file_id "platform FileRecord - NO path columns"
        string document_type "Layout | Agreement | Other"
        string title
        timestamptz created_at
        uuid created_by
        boolean is_deleted
    }

    SiteAssetSummary {
        uuid id PK
        uuid site_id FK "unique - 1:1 with site"
        int camera_count
        int dvr_count
        int nvr_count
        int hard_disk_count
        int switch_count
        int router_count
        int monitor_count
        string brand "optional"
        string model "optional"
        string remarks "optional"
        timestamptz created_at
        uuid created_by
        timestamptz updated_at "nullable"
        uuid updated_by "nullable"
        bytea row_version
    }
```

> **Mandated:** assets are **summary counts only** — there is deliberately **no Camera/Device entity** (freeze §7). Individual cameras are NOT tracked.

## Relationships

| Relationship | Cardinality | Type |
|--------------|-------------|------|
| Customer → Site | 1:N | Physical FK (same schema); site never moves between customers |
| Site → SiteContact | 1:0..3 | Composition; slot-constrained |
| Site → SiteAssetSummary | 1:1 | Composition; unique site_id |
| Site → SiteDocument | 1:N | Composition |
| Customer → platform user (portal login) | 0..1 | Logical reference |
| Site → AMCContract / Ticket / Invoice / ServiceSchedule | 1:N | **Logical** — owned by other schemas; site is the aggregation point (freeze §5) |

## Constraints & indexes

| Object | Definition |
|--------|-----------|
| `ux_customers_customer_number`, `ux_sites_site_number` | business numbers |
| `ck_site_contacts_contact_slot` | `contact_slot BETWEEN 1 AND 3` |
| `ux_site_contacts_site_id_contact_slot` | unique (site_id, contact_slot) — **DB-guaranteed max 3** |
| `ux_site_asset_summaries_site_id` | unique (site_id) — one summary per site |
| `ck_site_asset_summaries_counts_non_negative` | all `*_count >= 0` |
| `ix_sites_customer_id`, `ix_site_contacts_site_id`, `ix_site_documents_site_id` | child lookups |

## Domain events

| Event | Notes |
|-------|-------|
| CustomerCreated / Updated / Deactivated | audit |
| SiteCreated / Updated / Deactivated | audit |
| SiteContactAdded / Removed | audit |
| SiteAssetSummaryUpdated | audit |

Related: [entity-model.md §2.2](./entity-model.md) · [entity-lifecycle-matrix.md §2](./entity-lifecycle-matrix.md)
