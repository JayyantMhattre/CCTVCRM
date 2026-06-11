# ADR-Mobile-0006: Push notification architecture

## Status

Accepted (M4)

## Decision

Use **provider abstraction** (`PushNotificationProvider`, `LocalNotificationProvider`) with **FCM adapter** enabled via `--dart-define=ENABLE_FCM=true`. Default: `NoOpPushNotificationProvider`.

## Consequences

- No Firebase imports outside `fcm_push_provider.dart`
- CI builds without `google-services.json`
- Notification taps route through `DeepLinkHandler`

## References

- [notifications/README.md](../mobile/notifications/README.md)
