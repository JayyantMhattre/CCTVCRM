import 'dart:async';

import 'package:ashraak_mobile/core/notifications/notification_provider.dart';
import 'package:ashraak_mobile/core/notifications/notification_permissions.dart';

/// Orchestrates push + local providers — no vendor lock-in at call sites.
class NotificationService {
  NotificationService({
    required PushNotificationProvider pushProvider,
    required LocalNotificationProvider localProvider,
  })  : _push = pushProvider,
        _local = localProvider,
        permissions = NotificationPermissions(pushProvider);

  final PushNotificationProvider _push;
  final LocalNotificationProvider _local;
  final NotificationPermissions permissions;

  StreamSubscription<Map<String, String>>? _foregroundSub;
  void Function(Map<String, String> payload)? onNotificationTap;

  Future<void> initialize({
    void Function(Map<String, String> payload)? onTap,
  }) async {
    onNotificationTap = onTap;
    await _local.initialize();
    await _push.initialize();
    await permissions.ensureGranted();

    _foregroundSub = _push.onForegroundMessage.listen((payload) async {
      await _local.show(
        id: DateTime.now().millisecondsSinceEpoch.remainder(100000),
        title: payload['title'] ?? 'Ashraak',
        body: payload['body'] ?? 'New notification',
        payload: payload,
      );
    });

    _push.onNotificationOpened.listen((payload) {
      onNotificationTap?.call(payload);
    });
  }

  Future<String?> registerDeviceToken() => _push.getDeviceToken();

  Future<void> dispose() async {
    await _foregroundSub?.cancel();
    await _push.dispose();
  }
}
