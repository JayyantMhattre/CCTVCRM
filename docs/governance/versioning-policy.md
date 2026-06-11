# Versioning Policy

**Status:** Mandatory · **Phase:** R1 — Platform Freeze & Governance

Defines how the platform and everything in it is versioned. The baseline is **Platform V1 = `v1.0.0`**.

**Related:** [platform-freeze-policy.md](./platform-freeze-policy.md) · [upgrade-guide](../releases/upgrade-guide.md) · [architecture-decision-process.md](./architecture-decision-process.md)

---

## 1. Semantic Versioning (SemVer)

All versioned artifacts follow **`MAJOR.MINOR.PATCH`**:

| Part | Increment when… | Examples |
|------|------------------|----------|
| **MAJOR** | Breaking change (incompatible API/contract/behavior) | `v1.x.x → v2.0.0` |
| **MINOR** | Backward-compatible new capability (additive) | `v1.0.0 → v1.1.0` |
| **PATCH** | Backward-compatible bug/security fix | `v1.0.0 → v1.0.1` |

Pre-release/build metadata (`-rc.1`, `-beta`) is allowed for staging releases.

## 2. Platform versions

The **Platform version** describes the Core Platform as a whole.

| Version | Meaning |
|---------|---------|
| `v1.0.0` | **Frozen baseline** — the V1 release ([release notes](../releases/v1.0.0-release-notes.md)) |
| `v1.1.0` | Additive, governance-approved Core change (e.g., new storage provider, new theme adapter capability) — must clear the [extension policy](./platform-extension-policy.md) |
| `v1.0.1` | Bug/security/dependency/doc fix within the freeze ([allowed changes](./platform-freeze-policy.md#4-allowed-without-scope-expansion)) |
| `v2.0.0` | Breaking platform change — requires ADR, governance review, and a documented migration path |

Because Core is frozen, **MINOR platform bumps are rare and gated**; PATCH bumps cover the allowed maintenance changes; MAJOR is exceptional.

## 3. Theme versions

Themes are versioned **independently per adapter**.

- A theme adapter has its own SemVer (e.g., `hexadash-adapter v1.0.0`).
- **MINOR:** new themed surface/visual capability that stays within the 11 contracts.
- **MAJOR:** a change that alters contract expectations or requires consumer changes.
- Adding a **new theme adapter** is an additive Core change → typically a platform **MINOR** (allowed by the freeze). Onboard via the [theme guide](../frontend/themes/theme-onboarding-guide.md).
- Upgrading the underlying UI kit (CoreUI/Bootstrap major) is a dependency upgrade scoped to that adapter; re-run theme validation.

## 4. Module versions

Each **business module** ([business-module-policy.md](./business-module-policy.md)) carries its **own** SemVer and release cadence, independent of the platform.

- Modules declare the **minimum compatible platform version** they require (e.g., "requires Platform ≥ v1.0.0").
- A module MAJOR bump is internal to the module unless it changes a contract it publishes.
- Core Platform modules are versioned with the platform (they are frozen, so they move with platform PATCH/MINOR/MAJOR).

## 5. Mobile versions

The Flutter app follows [ADR-Mobile-0011 (versioning strategy)](../adr/ADR-Mobile-0011-versioning-strategy.md):

- App version (`MAJOR.MINOR.PATCH`) + build number for store submissions.
- Aligns with backend contract compatibility; the generated OpenAPI SDK is regenerated on backend contract changes ([ADR-Mobile-0005](../adr/ADR-Mobile-0005-openapi-sdk-generation.md)).
- Release/signing per [Mobile-0010](../adr/ADR-Mobile-0010-release-strategy.md)/[Mobile-0012](../adr/ADR-Mobile-0012-signing-strategy.md).

## 6. Backend versions

- The backend platform version tracks the **Platform version** (§2).
- API surface compatibility is the contract: additive endpoints/fields → MINOR; breaking request/response/contract changes → MAJOR (with migration notes).
- Runtime/framework upgrades (.NET, EF Core) follow the [upgrade-guide](../releases/upgrade-guide.md): one major at a time, central package management ([ADR-0010](../adr/ADR-0010-build-package-governance.md)).
- Database changes ship as EF Core migrations per module schema; never edit applied migrations.

## 7. Compatibility & upgrade requirements

| Change type | Version effect | Requirements |
|-------------|----------------|--------------|
| Bug/security fix | PATCH | Tests + docs; security may use expedited path |
| Dependency upgrade | PATCH or MINOR | One major at a time; build verified; docs updated |
| Additive Core capability | MINOR | Full [extension policy](./platform-extension-policy.md) gate + ADR |
| New theme adapter | Platform MINOR | [theme onboarding](../frontend/themes/theme-onboarding-guide.md) |
| Breaking change | MAJOR | ADR + governance review + **documented migration path** + deprecation window |

**Deprecation:** breaking changes provide a migration guide and (where feasible) a deprecation window with backward compatibility for at least one release.

## 8. Tagging & release notes

- Releases are git-tagged (`vMAJOR.MINOR.PATCH`).
- Each release updates/append release notes (mirroring [v1.0.0-release-notes.md](../releases/v1.0.0-release-notes.md)) and the [manifest](../releases/platform-v1-manifest.md) if inventory changed.
- The current baseline tag is **`v1.0.0`** (Platform Freeze).

---

**Summary:** SemVer everywhere; the frozen Core Platform moves conservatively (PATCH for maintenance, gated MINOR for additive, exceptional MAJOR for breaking); themes, business modules, and mobile version independently against a declared platform compatibility floor.
