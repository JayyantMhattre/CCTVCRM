import 'dart:io';

import 'package:ashraak_mobile/core/notifications/notification_provider.dart';

/// Platform permission helpers for push notifications.
class NotificationPermissions {
  NotificationPermissions(this._push);

  final PushNotificationProvider _push;

  Future<bool> ensureGranted() => _push.requestPermissions();

  String platformNotes() {
    if (Platform.isAndroid) {
      return 'Android 13+ requires POST_NOTIFICATIONS runtime permission.';
    }
    if (Platform.isIOS) {
      return 'iOS requires user authorization for alert, badge, and sound.';
    }
    return 'Push permissions are platform-specific.';
  }
}
