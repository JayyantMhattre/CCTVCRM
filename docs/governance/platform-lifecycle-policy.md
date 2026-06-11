# Platform Lifecycle Policy

**Status:** Mandatory · **Phase:** R1 — Platform Freeze & Governance

Defines the end-to-end lifecycle every approved platform change follows, and who is responsible at each stage. This is the execution pipeline behind the [freeze](./platform-freeze-policy.md), [extension](./platform-extension-policy.md), and [ADR](./architecture-decision-process.md) policies.

---

## 1. The lifecycle

```
Proposal → ADR → Review → Implementation → Documentation → Validation → Release
```

Every Core Platform change moves through all seven stages in order. A stage cannot start until the previous stage's exit criteria are met.

| # | Stage | Purpose | Exit criteria |
|---|-------|---------|---------------|
| 1 | **Proposal** | State the problem and intended direction; justify Core scope (or redirect to business/experimental) | Problem + reuse justification documented; classification confirmed ([module-classification](./module-classification-policy.md)) |
| 2 | **ADR** | Record the architectural decision | ADR drafted (`Proposed`) per [architecture-decision-process](./architecture-decision-process.md) |
| 3 | **Review** | Architecture + governance assessment against the [8 extension criteria](./platform-extension-policy.md) | ADR **Accepted**; version impact agreed ([versioning](./versioning-policy.md)) |
| 4 | **Implementation** | Build the change | Code complete; respects modular-monolith + contracts; no Core→business dependency |
| 5 | **Documentation** | Write/update docs | `docs/` updated; PR checklist complete ([documentation-governance](../documentation-governance.md)) |
| 6 | **Validation** | Prove correctness & non-regression | Tests pass; CI green; for themes, the [validation prompt](../frontend/themes/prompts/theme-validation-prompt.md) reports are produced |
| 7 | **Release** | Ship and tag | SemVer applied; release notes + manifest updated; git tag created ([versioning](./versioning-policy.md)) |

## 2. Stage detail

- **Proposal** — Default outcome for Core is rejection; most proposals become business/experimental modules. Include multi-product reuse evidence if targeting Core.
- **ADR** — Mandatory for the triggers in [architecture-decision-process §1](./architecture-decision-process.md#1-when-an-adr-is-required).
- **Review** — Checks fit, security, operability, and that the change is additive (MINOR) vs breaking (MAJOR). Assigns an operational owner (extension criterion 8).
- **Implementation** — Follows the relevant [extension guide](../extending/add-backend-module.md); no edits that violate the freeze; one major dependency at a time ([upgrade-guide](../releases/upgrade-guide.md)).
- **Documentation** — Code and docs ship together; ADR status updated to `Accepted`/implemented.
- **Validation** — Automated tests + CI; `tsc --noEmit` for web; manual/runtime validation where automated coverage can't reach.
- **Release** — Tag, notes, manifest; deprecation/migration notes for any breaking change.

## 3. Responsibilities (RACI)

| Stage | Author | Architecture Reviewer | Governance | Implementer/Owner | QA/CI |
|-------|:------:|:--------------------:|:----------:|:-----------------:|:-----:|
| Proposal | **R/A** | C | C | I | — |
| ADR | **R/A** | C | A | I | — |
| Review | C | **R** | **A** | C | — |
| Implementation | I | C | I | **R/A** | C |
| Documentation | C | C | I | **R/A** | C |
| Validation | I | C | I | C | **R/A** |
| Release | I | I | **A** | **R** | C |

R = Responsible, A = Accountable, C = Consulted, I = Informed.

**Named roles:**
- **Author** — proposes and drafts the ADR; supplies justification/evidence.
- **Architecture Reviewer(s)** — assess technical fit, risks, alternatives.
- **Governance** — owns Accept/Reject/Defer and guards the freeze + Core scope; accountable for release approval.
- **Implementer / Operational Owner** — builds the change and owns it long-term (maintenance, security, on-call).
- **QA/CI** — validation gates (tests, docs validation, build pipelines).

## 4. Fast paths & exceptions

| Situation | Path |
|-----------|------|
| Security fix | Expedited implementation; ADR/docs may be completed retroactively **within the same release cycle** ([freeze §6](./platform-freeze-policy.md#6-exception-path-security)) |
| New theme adapter | Skips the full Core-capability gate; uses the [theme onboarding lifecycle](../frontend/themes/theme-lifecycle.md) (additive, allowed by freeze) |
| Bug fix / docs / dependency patch | Normal review + tests; no ADR |
| Business module | Not subject to this Core lifecycle; uses the [business-module workflow](./business-module-policy.md) |

## 5. Traceability

Each change links: **Proposal → ADR → PR(s) → docs → tests → release tag**. The ADR is the anchor record; release notes and the [manifest](../releases/platform-v1-manifest.md) reflect the outcome.

---

**Summary:** Approved platform changes flow Proposal → ADR → Review → Implementation → Documentation → Validation → Release, with clear ownership at each stage. Security and theme-adapter additions have sanctioned fast paths; business modules follow their own workflow outside this Core pipeline.
