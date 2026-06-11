# Notification provider model

## PushNotificationProvider

| Method | Purpose |
|--------|---------|
| `initialize()` | Wire FCM listeners |
| `requestPermissions()` | OS permission prompt |
| `getDeviceToken()` | FCM token for backend |
| `onForegroundMessage` | Stream for in-app display |
| `onNotificationOpened` | Stream for tap → deep link |

## LocalNotificationProvider

Displays foreground alerts when push payload arrives while app is active.

## NotificationService

Single entry for app/features — composes push + local + permissions.
