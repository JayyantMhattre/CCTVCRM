# Feature flags — Architecture

```mermaid
flowchart LR
    Module[Module handler]
    Contract[IFeatureFlagService]
    Config[IOptions Features]
    Module --> Contract --> Config
```

## Design principles

- **Contract in SharedKernel** — modules depend on interface only
- **Host implementation** — swap for LaunchDarkly/Azure App Configuration later without module rewrites
- **Default deny** — unknown feature names return `false`

## Registration

`AddHostPlatformServices()` registers `IFeatureFlagService` → `ConfigFeatureFlagService`.

## Tenant overrides

Optional `TenantOverrides` dictionary keyed by tenant GUID (`D` or `N` format). Tenant-specific value wins over global `Flags`.
