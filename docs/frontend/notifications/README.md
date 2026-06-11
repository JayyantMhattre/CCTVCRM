# Notification preferences (frontend)

Per-user email notification toggle.

**Page:** `/users/me/notifications` → `NotificationPreferencesPage.tsx`

## Data

- Loads profile via `GET /api/v1/users/{userId}`
- Saves via `PATCH /api/v1/users/{userId}/preferences` with `{ emailNotificationsEnabled }`

## Backend note

The PATCH preferences endpoint is the documented client contract. If the API returns 404, the global error interceptor shows a toast with correlation ID — implement the backend route to persist changes.

## Related

- [settings-flow.md](./settings-flow.md)
