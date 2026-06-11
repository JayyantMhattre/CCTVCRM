# Push notifications — architecture

```mermaid
flowchart LR
    FCM[FCM/APNS]
    Push[PushNotificationProvider]
    Local[LocalNotificationProvider]
    Service[NotificationService]
    DeepLink[DeepLinkHandler]
    Router[GoRouter]

    FCM --> Push
    Push --> Service
    Service --> Local
    Service --> DeepLink
    DeepLink --> Router
```

- **No Firebase lock-in** at call sites — only `FcmPushNotificationProvider` references Firebase SDK.
- Foreground FCM messages display via local notifications.
- Background/tap events carry `deepLink` payload keys.
