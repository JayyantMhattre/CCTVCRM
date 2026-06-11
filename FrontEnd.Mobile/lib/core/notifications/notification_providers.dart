import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/notifications/fcm_push_provider.dart';
import 'package:ashraak_mobile/core/release/release_providers.dart';
import 'package:ashraak_mobile/core/notifications/flutter_local_notification_provider.dart';
import 'package:ashraak_mobile/core/notifications/noop_push_provider.dart';
import 'package:ashraak_mobile/core/notifications/notification_provider.dart';
import 'package:ashraak_mobile/core/notifications/notification_service.dart';

final pushNotificationProviderProvider = Provider<PushNotificationProvider>((ref) {
  final flavor = ref.watch(flavorConfigProvider);
  if (flavor.enableFcm && FcmPushNotificationProvider.isConfigured) {
    return FcmPushNotificationProvider();
  }
  return NoOpPushNotificationProvider();
});

final localNotificationProviderProvider = Provider<LocalNotificationProvider>(
  (ref) => FlutterLocalNotificationProvider(),
);

final notificationServiceProvider = Provider<NotificationService>((ref) {
  final service = NotificationService(
    pushProvider: ref.watch(pushNotificationProviderProvider),
    localProvider: ref.watch(localNotificationProviderProvider),
  );
  ref.onDispose(() => service.dispose());
  return service;
});
