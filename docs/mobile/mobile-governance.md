# Mobile governance

Extends [documentation-governance.md](../documentation-governance.md) for Flutter.

---

## Mandatory rules

1. **No feature without docs** — update `docs/mobile/` and manifest before merging UI code.
2. **Manifest must be updated** — [mobile-platform-manifest.md](./mobile-platform-manifest.md) on every capability change.
3. **ADR required** for architectural changes (navigation, state, offline, push, native hybrid).
4. **Reuse backend contracts** — same REST/OAuth; no mobile-only APIs without platform ADR.
5. **No duplicated DTOs** — generated SDK from OpenAPI ([sdk-generation.md](./sdk-generation.md)).
6. **Shared SDK generation** — API changes trigger Dart (and optionally TS) regen in same PR.
7. **Mobile modules mirror platform modules** — `features/` aligns with `docs/modules/`.

---

## Phase gates

| Phase | Allowed |
|-------|---------|
| **M0** | Docs, ADRs, manifest only |
| **M1** (complete) | `FrontEnd.Mobile/` foundation — core, routing, API client |
| **M2** (complete) | Auth, MFA, tenant context, profile foundation |
| **M3** (complete) | Files, sessions, settings, audit, notifications, profile enhancements |
| **M4** (complete) | Push, offline, biometrics, deep links, feature flags, crash/analytics |
| **M5** (complete) | Release engineering, flavors, Fastlane, store readiness |
| **M6** | Live store submission automation, invitations UI |
| **M4** | Push, offline, biometrics |

See [roadmap.md](./roadmap.md).

---

## PR checklist (mobile)

- [ ] `docs/mobile/mobile-platform-manifest.md` updated
- [ ] Feature doc or module-map row if new capability
- [ ] ADR linked if architecture decision
- [ ] OpenAPI/SDK regen if API touched
- [ ] `flutter analyze` / `flutter test` (when project exists)
- [ ] Doc validation passes

---

## Cross-platform sync

When backend capability changes:

1. Update `docs/modules/{module}/`
2. Update `docs/frontend/` if web impacted
3. Update mobile manifest + `docs/mobile/module-map.md`

---

## Automation

`doc-validation.json` includes `mobile-platform` mapping (docs-only until `FrontEnd.Mobile/` exists).
