# ADR-Mobile-0007: Offline cache architecture

## Status

Accepted (M4)

## Decision

**Online-first** read-through cache via `OfflineCache` abstraction; default implementation **Hive** (`HiveOfflineCache`). No offline write queue in M4.

## Consequences

- Repositories use `CachedRepositoryMixin` for profile/settings
- `OfflineBanner` + `SyncService` for retry/resume sync
- Write operations still require network

## References

- [offline/README.md](../mobile/offline/README.md)
- [background-sync/README.md](../mobile/background-sync/README.md)
