# Risk Register

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-8
**Review:** End of each sprint + phase gate

| ID | Category | Risk | Impact | Likelihood | Mitigation | Owner | Phase |
|----|----------|------|:------:|:----------:|------------|-------|:-----:|
| R01 | Technical | Lead conversion partial failure (multi-module) | H | M | Outbox + idempotent contracts; integration test; compensating actions documented | Backend lead | B1–B3 |
| R02 | Technical | Visit offline sync conflicts | H | M | Server-wins policy; `clientCorrelationId`; Sync Status UI; dedicated mobile tests | Mobile + B4 | Sprint 9 |
| R03 | Technical | PDF generation library choice delays B6 | M | M | D1 spike + ADR; fallback simple HTML print | Backend | D1/B6 |
| R04 | Technical | SMS provider not selected (DEP-02) | M | H | Abstract `ISmsProvider`; email fallback; decide Sprint 0 | Architect | D1 |
| R05 | Business | Scope creep beyond freeze | H | M | Change request process §22; gate reviews | PM | All |
| R06 | Technical | Platform code modification pressure | H | L | Architecture tests; freeze policy; PR checklist | Tech lead | All |
| R07 | Operational | Audit read API stub limits admin forensics | L | H | Business history entities; Mongo query Phase 2 optional | Backend | B5+ |
| R08 | Data | Migration failure in production | H | L | Additive migrations; backup; forward-fix; rollback runbook | DevOps | REL |
| R09 | Mobile | App store rejection (permissions/privacy) | M | L | Reuse platform store-readiness; privacy manifest review | Mobile | Sprint 9 |
| R10 | Infrastructure | PostgreSQL performance on visit photos metadata | M | M | Indexing plan; pagination; archive policy future | Backend | B4 |
| R11 | Business | UAT finds gap vs freeze interpretation | M | M | UAT script from freeze §2; traceability matrix in tests | BA | Sprint 11 |
| R12 | Technical | OpenAPI/SDK drift web vs mobile | M | M | CI drift check; regen in same PR as API change | DevOps | D1+ |
| R13 | Operational | Engineer adoption of evidence checklist | M | M | Mobile-first UX; training; hard server validation | Product | Sprint 9 |
| R14 | Technical | AMC schedule generation edge cases (frequency) | M | M | Unit tests per frequency; manual QA calendar | Backend | B3–B4 |
| R15 | Infrastructure | Redis/Mongo dependency outage | M | L | Platform ops runbooks; CCTV degrades gracefully (no hard dep for reads) | Ops | REL |
| R16 | Data migration | No legacy data — greenfield CCTV | L | — | N/A — new schemas only; no ETL V1 | — | — |
| R17 | Mobile | Offline queue data loss on app kill | H | M | Persistent local storage (platform offline); retry on launch | Mobile | Sprint 9 |
| R18 | Business | Invoice Option B vs BR-INV-02 wording | L | H | Documented Option B in design; server implements D0-4 decision | Backend | B6 |
| R19 | Technical | Cross-team bottleneck on shared API changes | M | M | Contract-first SharedKernel; OpenAPI review in PR | Tech lead | All |
| R20 | Operational | Insufficient QA time | M | M | Automate early; Sprint 11 buffer; prioritize E2E paths | QA lead | Sprint 10–11 |

---

## Risk heat map

|  | Low impact | Medium impact | High impact |
|--|----------|---------------|-------------|
| **High likelihood** | R07, R18 | R04, R05 | — |
| **Medium likelihood** | R09, R16 | R03, R10, R11, R12, R13, R14, R19, R20 | R01, R02 |
| **Low likelihood** | — | R15 | R06, R08 |

---

## Top 5 risks (priority)

1. **R01** — Lead conversion orchestration  
2. **R02** — Engineer offline sync  
3. **R05** — Scope creep  
4. **R06** — Platform modification  
5. **R04** — SMS provider delay  

---

## Mitigation tracking

Update status in sprint retro:

| Status | Meaning |
|--------|---------|
| Open | Not mitigated |
| Mitigating | Actions in progress |
| Monitoring | Controls in place |
| Closed | Risk materialized and resolved OR no longer applicable |

---

Related: [implementation-roadmap.md](./implementation-roadmap.md) · [phase-execution-playbook.md](./phase-execution-playbook.md)
