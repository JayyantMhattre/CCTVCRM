# ERD — Lead Domain

**Schema:** `cctv_lead` · **Module:** Lead Management (2)
**Source of truth:** [requirements-freeze-v1.md §10](../requirements-freeze-v1.md) · Rules: BR-LEAD-01..03

---

## ER diagram

```mermaid
erDiagram
    Lead ||--o{ LeadActivity : "logs"
    Lead ||--o{ LeadRemark : "has"
    Lead ||--o{ LeadAttachment : "has"

    Lead {
        uuid id PK
        string lead_number UK "LD-YYYY-NNNN"
        string source "WebsiteContact | AmcInquiry | GetQuote | Manual"
        string status "New..Converted (BR-LEAD-01)"
        string contact_name
        string organization_name "nullable"
        string email
        string phone
        string city
        string address "nullable"
        string requirement_summary "nullable"
        uuid owner_user_id "admin owner - logical ref auth"
        uuid converted_customer_id "nullable - logical ref cctv_customer"
        uuid converted_site_id "nullable - logical ref cctv_customer"
        uuid converted_contract_id "nullable - logical ref cctv_amc"
        timestamptz converted_at "nullable"
        timestamptz created_at
        uuid created_by "system user for website leads"
        timestamptz updated_at "nullable"
        uuid updated_by "nullable"
        boolean is_deleted
        bytea row_version
    }

    LeadActivity {
        uuid id PK
        uuid lead_id FK
        string activity_type "StatusChange | Call | Email | Meeting | QuotationSent | Note"
        string from_status "nullable"
        string to_status "nullable"
        string description
        timestamptz occurred_at
        timestamptz created_at
        uuid created_by
    }

    LeadRemark {
        uuid id PK
        uuid lead_id FK
        string remark
        timestamptz created_at
        uuid created_by
    }

    LeadAttachment {
        uuid id PK
        uuid lead_id FK
        uuid file_id "platform FileRecord - NO path columns"
        string title
        timestamptz created_at
        uuid created_by
        boolean is_deleted
    }
```

## Relationships

| Relationship | Cardinality | Type |
|--------------|-------------|------|
| Lead → LeadActivity | 1:N | Composition (physical FK, append-only) |
| Lead → LeadRemark | 1:N | Composition (physical FK, append-only) |
| Lead → LeadAttachment | 1:N | Composition (physical FK) |
| Lead → Customer / Site / AMCContract (conversion) | 0..1 each | **Logical** cross-schema references set atomically at conversion (BR-LEAD-03) |
| LeadAttachment → FileRecord | N:1 | **Logical** platform reference (`file_id`) |

## Constraints & indexes

| Object | Definition |
|--------|-----------|
| `ux_leads_lead_number` | unique (lead_number) |
| `ck_leads_status` | status ∈ frozen list (BR-LEAD-01) |
| `ck_leads_source` | source ∈ {WebsiteContact, AmcInquiry, GetQuote, Manual} |
| `ix_leads_status`, `ix_leads_created_at` | pipeline queries |
| `ix_lead_activities_lead_id`, `ix_lead_remarks_lead_id`, `ix_lead_attachments_lead_id` | child lookups |
| Conversion consistency | `converted_*` columns all set together with status=Converted (application invariant) |

## Domain events (→ platform Audit / Notifications / Webhooks)

| Event | Triggers |
|-------|----------|
| LeadCreated | Notification "Lead Created" (freeze §17); audit |
| LeadStatusChanged | Activity row; audit |
| LeadConverted | Creates Customer + Site + Initial Contract via module contracts (outbox); Notification "Lead Converted"; audit |

Related: [entity-model.md §2.1](./entity-model.md) · [entity-lifecycle-matrix.md §1](./entity-lifecycle-matrix.md) · [workflow-overview.md §1](../workflow-overview.md)
