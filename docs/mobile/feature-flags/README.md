# Mobile feature flags (M4)

**Path:** `lib/core/feature_flags/`

## Provider

`MobileFeatureFlagProvider` implements `FeatureFlagService`:

1. **Defaults** — `defaultMobileFlags` (aligned with backend naming)
2. **Remote** — `GET /api/v1/platform/feature-flags` (graceful 404 fallback)
3. **Cached** — Hive via `OfflineCache`
4. **Tenant-aware** — JWT `tenantId` + tenant overrides map

## Keys

| Flag | Key |
|------|-----|
| Push | `mobile.push` |
| Biometrics | `mobile.biometrics` |
| Offline | `mobile.offline` |
| Deep links | `mobile.deep-links` |
| Files | `mobile.files` |
| Beta | `mobile.beta` |

Usage: `ref.watch(featureFlagEnabledProvider(MobileFeatureFlags.biometrics))`
