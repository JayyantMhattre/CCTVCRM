# Platform Extension Policy

**Status:** Mandatory · **Phase:** R1 — Platform Freeze & Governance

Defines the **bar a capability must clear to enter the frozen Core Platform**. The default answer to "should this be in Core?" is **no** — Core is frozen ([platform-freeze-policy.md](./platform-freeze-policy.md)). This policy is how the rare, justified exception is approved.

**Related:** [module-classification-policy.md](./module-classification-policy.md) · [architecture-decision-process.md](./architecture-decision-process.md) · [platform-lifecycle-policy.md](./platform-lifecycle-policy.md)

---

## The 8 entry criteria (ALL required)

A capability may enter Core Platform **only if it satisfies every one** of:

| # | Criterion | What it means | Evidence |
|---|-----------|---------------|----------|
| 1 | **Used by multiple products** | Demonstrable reuse across ≥ 2 products/tenplates, not a single product's need | Usage/demand from multiple consumers |
| 2 | **Domain independent** | No business-vertical assumptions (not CRM/billing/LMS/etc.) | Capability description is domain-neutral |
| 3 | **Not business specific** | Generic capability, not a product workflow | Fails if it encodes a specific business process |
| 4 | **Has an ADR** | Architecture Decision Record accepted | Link to merged ADR ([process](./architecture-decision-process.md)) |
| 5 | **Has architecture review** | Reviewed for fit with modular-monolith + contracts + theme/abstraction patterns | Review sign-off |
| 6 | **Has documentation** | Module/API/ops docs per governance | `docs/` pages + PR checklist |
| 7 | **Has tests** | Adequate automated coverage | Passing tests in CI |
| 8 | **Has operational ownership** | A named owner accountable for maintenance, security, on-call | Owner recorded in module README |

If **any** criterion is unmet → **not Core**. Redirect to a [business module](./business-module-policy.md) or keep it [experimental](./module-classification-policy.md#3-experimental-modules).

---

## Decision matrix

Use this to classify a proposed capability. Score each dimension, then apply the rule.

| Dimension | Core Platform | Business Module | Experimental |
|-----------|---------------|-----------------|--------------|
| Reuse across products | Multiple (required) | Single product | Unknown / exploratory |
| Domain coupling | None | High (vertical-specific) | Varies |
| Maturity | Proven, stable | Product-ready | Prototype |
| ADR | Required (accepted) | Optional | Only to graduate |
| Architecture review | Required | Tech-lead review | Light |
| Docs | Required | Required | Minimal |
| Tests | Required | Required | Optional |
| Operational owner | Required | Product team | Author |
| Stability commitment | Frozen / SemVer-strict | Per product | None |
| **Outcome** | **Enter Core (rare)** | **Build as business module** | **Sandbox behind flag** |

**Rule:** Core requires a clean sweep of the Core column **and** all 8 entry criteria. A single "Business" or "Experimental" signal disqualifies Core entry.

### Worked examples

| Proposal | Multiple products? | Domain-independent? | Verdict |
|----------|--------------------|--------------------|---------|
| A new theme adapter | Yes | Yes | ✅ Allowed by freeze (additive) — onboard via [theme guide](../frontend/themes/theme-onboarding-guide.md) |
| New storage provider for Files | Yes | Yes | ✅ Core-eligible (ADR + review) — extends existing Core capability |
| New auth mechanism (e.g., passkeys) | Yes | Yes | ✅ Core-eligible (ADR + security review) |
| "Lead scoring" engine | No | No (CRM-specific) | ❌ Business module (CRM) |
| Billing/invoicing | No | No | ❌ Business module (Billing) |
| Workflow/automation engine | Maybe | Partially | ⚠️ Start Experimental; graduate only if it proves multi-product, domain-independent reuse |
| AI/RAG assistant | Maybe | Maybe | ⚠️ Experimental behind a flag first |

---

## Extending an existing Core capability vs. adding a new one

- **Extending** (e.g., another Files backend, another notification channel, another theme adapter): still requires ADR + review + docs + tests, but reuses the existing contract/abstraction — lower risk, often allowed.
- **Adding a brand-new Core capability**: highest scrutiny; must clear all 8 criteria and the matrix; almost always preferable to ship as a business/experimental module first and graduate.

---

## Process

1. Author a proposal (problem, scope, why Core, reuse evidence).
2. Open an **ADR** ([architecture-decision-process.md](./architecture-decision-process.md)).
3. Architecture + governance review against the 8 criteria and the matrix.
4. If approved, implement through the [platform lifecycle](./platform-lifecycle-policy.md) with docs + tests + named owner.
5. Version per [versioning-policy.md](./versioning-policy.md) (additive Core change → minor; breaking → major).

**Default outcome is rejection from Core.** This is intentional: it keeps the platform lean and reusable.
