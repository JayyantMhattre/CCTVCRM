# Mobile push notifications (M4)

Vendor-neutral notification stack in `FrontEnd.Mobile/lib/core/notifications/`.

## Components

| Type | Interface | Default | FCM |
|------|-----------|---------|-----|
| Push | `PushNotificationProvider` | `NoOpPushNotificationProvider` | `FcmPushNotificationProvider` |
| Local | `LocalNotificationProvider` | `FlutterLocalNotificationProvider` | — |
| Orchestrator | `NotificationService` | Riverpod `notificationServiceProvider` | — |

## Enable FCM

Build with `--dart-define=ENABLE_FCM=true` and add platform Firebase config (`google-services.json`, `GoogleService-Info.plist`).

Without config, push stays no-op — CI safe.

## Deep links

Notification tap payloads route through `DeepLinkHandler` → existing GoRouter paths.

## Docs

- [architecture.md](./architecture.md)
- [setup.md](./setup.md)
- [provider-model.md](./provider-model.md)
- [permissions.md](./permissions.md)

ADR: [ADR-Mobile-0006](../adr/ADR-Mobile-0006-push-notifications.md)
