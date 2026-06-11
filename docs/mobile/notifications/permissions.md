# Notification permissions

| Platform | Requirement |
|----------|-------------|
| Android 13+ | `POST_NOTIFICATIONS` runtime permission |
| iOS | User authorization (alert, badge, sound) |

`NotificationPermissions.ensureGranted()` delegates to the active `PushNotificationProvider`.

Denied permissions: push token may be null; in-app features unaffected.
