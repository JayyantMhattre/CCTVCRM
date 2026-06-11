# Push notifications — setup

## Development (no FCM)

Default build — `NoOpPushNotificationProvider`, local notifications still initialize.

## Production FCM

1. Create Firebase project and add Android/iOS apps (`com.ashraak`).
2. Place `google-services.json` in `android/app/`.
3. Place `GoogleService-Info.plist` in `ios/Runner/`.
4. Build: `flutter run --dart-define=ENABLE_FCM=true`

## Backend token registration

Device token registration endpoint is **future** — `NotificationService.registerDeviceToken()` returns token for host registration when API is exposed.
