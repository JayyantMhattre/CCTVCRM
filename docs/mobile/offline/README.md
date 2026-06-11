# Offline cache (M4)

**Strategy:** Online-first with read-through cache.

**Path:** `lib/core/offline/`

## Storage

`HiveOfflineCache` implements `OfflineCache` — single abstraction, no Hive spread in features.

## Cached modules

| Data | Cache key |
|------|-----------|
| Profile | `profile:{userId}` |
| Tenant | `tenant:{tenantId}` |
| Settings | `settings:{tenantId}` |
| Notification prefs | `notification_prefs:{userId}` |
| Files metadata | `files_metadata:{userId}` |

## UX

`OfflineBanner` — offline indicator, last sync time, retry → `SyncService`.

ADR: [ADR-Mobile-0007](../adr/ADR-Mobile-0007-offline-cache.md)
