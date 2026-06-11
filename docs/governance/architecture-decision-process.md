# Architecture Decision Process

**Status:** Mandatory · **Phase:** R1 — Platform Freeze & Governance

Defines **when** an Architecture Decision Record (ADR) is required and the **workflow** for authoring, reviewing, and accepting one. ADRs are the governance gate for all significant change to the frozen platform.

**Related:** [platform-extension-policy.md](./platform-extension-policy.md) · [platform-freeze-policy.md](./platform-freeze-policy.md) · [platform-lifecycle-policy.md](./platform-lifecycle-policy.md) · [ADR catalog](../adr/) · [ADR template](../adr/ADR-0000-template.md)

---

## 1. When an ADR is required

An ADR is **mandatory** for any of the following:

| Trigger | Examples |
|---------|----------|
| **New platform capability** | A new Core module/service clearing the [extension policy](./platform-extension-policy.md) |
| **New Theme Engine capability** | A new theme **contract**, or a structural change to the adapter architecture (a new *theme adapter* uses the lighter [theme onboarding](../frontend/themes/theme-onboarding-guide.md), but changing the contracts needs an ADR) |
| **New authentication mechanism** | Passkeys/WebAuthn, new OAuth flow, new identity provider strategy |
| **New storage provider** | New `Files` backend, new persistence technology |
| **New integration framework** | New outbound/inbound integration pattern, message broker, event bus |
| **Breaking change** | Any MAJOR version change to a contract, API, or behavior ([versioning-policy](./versioning-policy.md)) |
| **Cross-cutting infrastructure change** | Observability, caching, outbox, host hardening, multi-region |
| **Reclassification to Core** | Promoting an experimental/business capability into Core |

An ADR is **not required** (normal review suffices) for: routine bug fixes, security patches (ADR may be retroactive per the freeze policy), documentation, dependency patch/minor upgrades that don't change architecture, and adding a new theme **adapter** (additive, governed by theme docs).

> Rule of thumb: if the change affects **structure, contracts, cross-cutting behavior, or the Core scope**, it needs an ADR.

## 2. ADR format

Use the [ADR template](../adr/ADR-0000-template.md). Numbering follows the existing catalog conventions:

- Platform: `ADR-00NN-<slug>.md`
- Domain-scoped: `ADR-<Area>-00NN-<slug>.md` (e.g., `ADR-Webhook-0003-...`, `ADR-ApiKeys-0001-...`, `ADR-Mobile-0007-...`)

Each ADR states: **Context**, **Decision**, **Status**, **Consequences** (and alternatives considered). Status lifecycle: `Proposed → Accepted → (Superseded | Deprecated)`.

## 3. ADR workflow

```
Proposal ─▶ Draft ADR (Proposed) ─▶ Architecture Review ─▶ Governance Decision
                                                              ├─ Accepted  ─▶ Implementation (lifecycle)
                                                              ├─ Rejected  ─▶ recorded with rationale
                                                              └─ Deferred  ─▶ revisit later
```

1. **Proposal** — author raises a problem + intended direction (and why Core, if applicable, per the [extension policy](./platform-extension-policy.md) 8 criteria).
2. **Draft ADR** — create `ADR-XXXX-<slug>.md` in `docs/adr/` with status **Proposed**.
3. **Architecture review** — reviewers assess fit with the modular monolith, contracts, theme/abstraction patterns, security, and operability.
4. **Governance decision** — **Accepted**, **Rejected** (recorded with reasons), or **Deferred**.
5. **Implementation** — proceeds through the [platform lifecycle](./platform-lifecycle-policy.md) only after acceptance.
6. **Supersession** — a later ADR may supersede an earlier one; mark the old ADR `Superseded by ADR-YYYY`.

## 4. Linkage to other governance

- An accepted ADR is a **prerequisite** for entering Core ([extension policy](./platform-extension-policy.md), criterion 4).
- ADRs determine version impact ([versioning-policy](./versioning-policy.md)): breaking decision → MAJOR; additive → MINOR.
- Every ADR-driven change ships with documentation ([documentation-governance](../documentation-governance.md)) and tests; CI [docs validation](../../.github/workflows/docs-validation.yml) enforces docs.

## 5. Responsibilities

| Role | Responsibility |
|------|----------------|
| **Author** | Writes the proposal + ADR; supplies reuse/justification evidence |
| **Architecture reviewer(s)** | Assess technical fit, risks, alternatives |
| **Governance** | Final Accept/Reject/Defer; guards Core scope and the freeze |
| **Implementer / owner** | Builds per the ADR; provides docs, tests, operational ownership |

---

**Summary:** Structural, contract-level, cross-cutting, Core-scope, or breaking changes require an accepted ADR before implementation. The workflow is Proposal → ADR → review → decision → implementation, tightly linked to the extension, versioning, and lifecycle policies.
