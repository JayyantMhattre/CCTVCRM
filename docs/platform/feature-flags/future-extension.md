# Feature flags — Future extension

## Planned (not implemented)

| Capability | Approach |
|------------|----------|
| Plan-based flags | Resolve tenant plan via `ITenantService`, merge with config |
| Redis cache | Use `CacheKeyBuilder.ForFeatureFlags(tenantId)` |
| Admin UI | Tenant module endpoints to toggle flags |
| External provider | New `IFeatureFlagService` implementation in Infrastructure |

## Migration path

1. Keep `IFeatureFlagService` signature stable
2. Add `FeatureFlagService` implementation package
3. Register conditionally in `HostPlatformExtensions` from `Features:Provider` config

## Do not

- Introduce heavy third-party SDKs without ADR
- Hard-code feature checks in host — keep in modules behind contract
