# Background sync (M4)

**Path:** `lib/core/sync/`

## Triggers (no aggressive polling)

- App resume (`BackgroundSyncCoordinator`)
- Connectivity restored
- Manual retry from offline banner

## Targets

Profile, tenant, settings, notification preferences, files metadata timestamps.

## Network awareness

`ConnectivityService` — skips sync when offline.

Battery-friendly: no background timer polling.
