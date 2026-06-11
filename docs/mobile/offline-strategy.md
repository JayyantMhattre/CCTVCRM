# Offline strategy

## M4: **online-first with read cache**

Read-through cache for profile, tenant, settings, notification prefs, and files metadata. Writes still require network. See [offline/README.md](./offline/README.md).

---

## Rationale

- SaaS identity and tenant operations need authoritative server state.
- Reduces conflict complexity for MFA, sessions, and audit.

---

## Future phases (M4+)

| Capability | Approach |
|------------|----------|
| Read cache | Hive/SQLite cache of last-fetched lists (audit, users) |
| Write queue | Outbox pattern on device for non-critical actions only |
| Conflict handling | Server wins; surface ProblemDetails to user |
| Sync indicator | Global connectivity banner + per-screen stale badges |

---

## What may be cached (future)

| Data | Cache? | Notes |
|------|--------|-------|
| User profile | Yes | Short TTL |
| Tenant settings | Yes | Invalidate on PATCH |
| Audit logs | Yes | Page-based, read-only |
| Auth tokens | Secure storage | Not "offline mode" |
| File blobs | Optional | Encrypted temp dir; explicit user download |

---

## Implementation gate

Offline support requires:

1. ADR for sync model
2. Manifest update
3. Backend idempotency keys (if write queue)

No implementation in M0.
