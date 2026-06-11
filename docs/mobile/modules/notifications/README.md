# Mobile — Notification preferences

**Feature:** `FrontEnd.Mobile/lib/features/notifications/`

## API

| Action | Method | Path |
|--------|--------|------|
| Read prefs | `GET` | `/api/v1/users/{userId}` → `preferences` |
| Update email toggle | `PATCH` | `/api/v1/users/{userId}/preferences` |

Body (M3):

```json
{ "emailNotificationsEnabled": true }
```

Self-only — `userId` must match signed-in user (from JWT `sub`).

## Provider

`NotificationsProvider` — loads preferences, saves email toggle.

## Route

`/notifications/preferences`

## Web parity

`FrontEnd/apps/web/src/modules/users/pages/NotificationPreferencesPage.tsx`
