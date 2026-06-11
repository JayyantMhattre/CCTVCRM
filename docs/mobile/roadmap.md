# Mobile roadmap

High-level delivery phases. **M0 is documentation only.**

---

## M0 — Platform foundation (complete)

- [x] Mobile docs root (`docs/mobile/`)
- [x] Platform manifest
- [x] Governance + ADRs (Flutter, Riverpod, GoRouter)
- [x] SDK / API / security / release strategy

---

## M1 — Flutter foundation (complete)

- [x] `FrontEnd.Mobile/` Flutter project (`app/`, `core/`, `shared/`, `features/` scaffold)
- [x] Riverpod global providers, GoRouter shell, Dio `BaseApiClient`
- [x] Auth token infra + secure storage (no login UI)
- [x] Environments, correlation, logging, theme, placeholder shell
- [x] OpenAPI → Dart SDK **tooling** (`scripts/generate-mobile-sdk.*`)
- [x] Unit + widget test foundation
- [x] GitHub Actions `mobile.yml` (analyze, test, debug Android/iOS builds)
- [x] `docs/mobile/foundation/`

**Manifest:** Foundation ✓, Correlation ✓, Auth → In Progress (infra only)

**Deferred to M2:** Login/register UI, first generated SDK commit, OpenAPI drift CI gate

---

## M2 — Tenant & identity UX

- Tenant profile + settings
- User profile + preferences
- Sessions list/revoke
- MFA login challenge
- Dashboard shell + guards

**Manifest:** Tenant, Users, Sessions, MFA → In Progress

---

## M3 — Core platform modules (complete)

- [x] Files: upload (camera/gallery/picker), download, preview, delete
- [x] Notification preferences (email toggle)
- [x] Sessions: list, revoke, revoke all
- [x] Tenant settings (read-only for non-admins)
- [x] Audit viewer (admin, paging + filters)
- [x] Profile enhancements (avatar, tenant, roles, shortcuts)
- [x] `docs/mobile/modules/`
- [x] Unit + widget tests

**Manifest:** Files, Notifications, Sessions, Audit, Tenant Settings → Done

**Deferred to M4:** Invitations, push, offline, biometrics, deep links

---

## M4 — Production capabilities (complete)

- [x] Push notifications (FCM adapter + local notifications)
- [x] Biometric app unlock (`local_auth`)
- [x] Deep linking (App Links / custom scheme)
- [x] Offline read cache (Hive) + background sync
- [x] Mobile feature flags (remote + cached + tenant-aware)
- [x] Crash reporting abstraction
- [x] Analytics abstraction
- [x] Production docs + ADRs 0006–0009

**Manifest:** Push, Biometrics, Offline, Feature Flags, Deep Links → Done

## M5 — Release engineering (complete)

- [x] Build flavors (dev, qa, uat, prod) Android + iOS
- [x] `version.yaml` + sync scripts
- [x] Android signing strategy + CI secrets
- [x] iOS signing + ExportOptions template
- [x] Fastlane lanes (android/ios)
- [x] `android-release.yml`, `ios-release.yml`, flavor matrix in `mobile.yml`
- [x] `store-assets/` + store readiness docs
- [x] Release docs + ADRs 0010–0012

**Manifest:** Release Engineering ✓, Store Readiness ✓

## M6 — Growth

- Invitations accept flow UI
- Remote feature-flag HTTP endpoint (host)
- Play/App Store live submission automation

---

## Success criteria

Mobile is **first-class** when:

- Every backend module in manifest has a planned or Done mobile row
- SDK generation is CI-gated
- No hand-written API DTOs in `lib/features/`
