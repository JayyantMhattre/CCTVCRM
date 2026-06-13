# ERD — AMC Domain

**Schema:** `cctv_amc` · **Modules:** AMC Plans (6), AMC Contracts (7)
**Source of truth:** [requirements-freeze-v1.md §8–§9](../requirements-freeze-v1.md) · Rules: BR-AMC-01..08

---

## ER diagram

```mermaid
erDiagram
    AMCPlan ||--|{ AMCPlanVersion : "versioned (BR-AMC-06)"
    AMCContract ||--|{ AMCContractTerm : "renewal history (BR-AMC-01)"
    AMCPlanVersion ||..o{ AMCContractTerm : "pinned version (BR-AMC-07)"
    AMCContract ||--o{ AMCContractDocument : "PDFs and signed copies"
    AMCContractTerm |o--o{ AMCContractDocument : "term-specific docs"

    AMCPlan {
        uuid id PK
        string plan_code UK "SILVER | GOLD | PLATINUM ..."
        string name
        string description "nullable"
        string status "Active | Retired"
        timestamptz created_at
        uuid created_by
        timestamptz updated_at "nullable"
        uuid updated_by "nullable"
        boolean is_deleted
        bytea row_version
    }

    AMCPlanVersion {
        uuid id PK
        uuid amc_plan_id FK
        int version_no "unique per plan"
        numeric price "numeric(18,2) - decimal only"
        int visit_frequency_per_year "drives schedule generation (BR-SCHED-02)"
        string included_services "frozen at version"
        string sla_terms "frozen at version"
        date effective_from
        string status "Draft | Published | Superseded"
        timestamptz created_at
        uuid created_by
    }

    AMCContract {
        uuid id PK
        string contract_number UK "AMC-YYYY-NNNN"
        uuid site_id "logical ref cctv_customer - ONE ACTIVE per site"
        uuid customer_id "logical ref cctv_customer"
        uuid source_lead_id "nullable - initial contract from conversion"
        string status "Active | Expired | Cancelled"
        timestamptz created_at
        uuid created_by
        timestamptz updated_at "nullable"
        uuid updated_by "nullable"
        bytea row_version
    }

    AMCContractTerm {
        uuid id PK
        uuid amc_contract_id FK
        int term_no "unique per contract - Term 2026, 2027..."
        uuid amc_plan_version_id FK "pins price-frequency-SLA"
        date start_date
        date end_date
        numeric agreed_price "numeric(18,2)"
        string status "Draft | Active | Expired | Cancelled"
        string origin "New | Renewal"
        boolean renewal_requested_by_customer "BR-AMC-08"
        timestamptz renewal_requested_at "nullable"
        timestamptz created_at
        uuid created_by
        timestamptz updated_at "nullable"
        uuid updated_by "nullable"
        bytea row_version
    }

    AMCContractDocument {
        uuid id PK
        uuid amc_contract_id FK
        uuid amc_contract_term_id FK "nullable - term-specific"
        uuid file_id "platform FileRecord - NO path columns"
        string document_type "ContractPdf | SignedCopy | Other"
        string title
        timestamptz created_at
        uuid created_by
    }
```

## Relationships

| Relationship | Cardinality | Type |
|--------------|-------------|------|
| AMCPlan → AMCPlanVersion | 1:N | Composition; versions append-only, immutable once referenced (BR-AMC-07) |
| AMCContract → AMCContractTerm | 1:N | Composition; **permanent renewal history** — never deleted (BR-AMC-01) |
| AMCPlanVersion → AMCContractTerm | 1:N | Physical FK (same schema); pins commercial terms |
| AMCContract → Site / Customer | N:1 | **Logical** cross-schema references |
| AMCContractDocument → FileRecord | N:1 | **Logical** platform reference (`file_id`) |

## Constraints & indexes

| Object | Definition |
|--------|-----------|
| `ux_amc_contracts_site_id_active` | unique (site_id) **WHERE status = 'Active'** — one active contract per site (BR-AMC-02) |
| `ux_amc_contract_terms_contract_id_active` | unique (amc_contract_id) **WHERE status = 'Active'** — one active term |
| `ux_amc_plan_versions_plan_id_version_no` | unique (amc_plan_id, version_no) |
| `ux_amc_contract_terms_contract_id_term_no` | unique (amc_contract_id, term_no) |
| `ck_amc_contract_terms_dates` | `end_date > start_date` |
| `ix_amc_contract_terms_end_date` | AMC Expiry Reminder scans (freeze §17) |
| Visibility rule | Customer queries filter to the **active term** only (BR-AMC-03/04) — query-side, not schema-side |

## Domain events

| Event | Notes |
|-------|-------|
| PlanCreated / PlanVersionPublished / PlanRetired | audit |
| ContractCreated (incl. from lead conversion) | audit |
| TermCreated (origin New/Renewal) / TermActivated / TermExpired | audit; TermActivated triggers schedule auto-generation (BR-SCHED-02) |
| RenewalRequested (by customer) | audit; admin work queue |
| AmcExpiryReminderDue | Notification "AMC Expiry Reminder" (freeze §17) |
| ContractPdfGenerated | document row + platform Files (freeze §19) |

Related: [entity-model.md §2.3](./entity-model.md) · [entity-lifecycle-matrix.md §3](./entity-lifecycle-matrix.md) · [workflow-overview.md §2](../workflow-overview.md)
