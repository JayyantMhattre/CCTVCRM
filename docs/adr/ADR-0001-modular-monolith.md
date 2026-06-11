# ADR-0001: Modular Monolith

**Status:** Accepted  
**Date:** 2026-05-01

---

## Context

Ashraak targets teams building multi-tenant SaaS products who need fast iteration, clear boundaries, and a path to scale without immediate microservices complexity.

---

## Decision

Ship as a **single deployable API** (`Ashraak.Api`) composed of **vertical slice modules** (Auth, Tenant, Users, Audit, Caching) with:

- Four layers per module (Domain, Application, Infrastructure, Api)
- Integration only via `SharedKernel.Contracts`
- Schema-isolated PostgreSQL per SQL module
- Observer modules at Layer 3 (Audit)

---

## Rationale

| Benefit | Explanation |
|---------|-------------|
| Operational simplicity | One process, one deployment artifact |
| Strong boundaries | Project references + architecture tests |
| Template clarity | New products copy module pattern |
| Future extraction | Modules can become services later with contract events |

---

## Consequences

**Positive:** Fast local dev, shared transaction boundaries where needed, unified middleware.

**Negative:** All modules scale together; blast radius of defects is process-wide. Requires discipline to avoid boundary erosion.

---

## Alternatives considered

| Alternative | Rejected because |
|-------------|------------------|
| Microservices from day one | High ops cost for template users |
| Traditional layered monolith | Poor feature isolation and rename cost |
| NuGet-packaged modules only | Harder to fork and customize |

---

## References

- [modular-monolith.md](../architecture/modular-monolith.md)
- `BackEnd/src/Host/Ashraak.Api/Extensions/ModuleExtensions.cs`
