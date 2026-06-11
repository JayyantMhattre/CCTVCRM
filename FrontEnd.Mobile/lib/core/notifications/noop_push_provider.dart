import 'dart:async';

import 'package:ashraak_mobile/core/notifications/notification_provider.dart';

class NoOpPushNotificationProvider implements PushNotificationProvider {
  final _opened = StreamController<Map<String, String>>.broadcast();
  final _foreground = StreamController<Map<String, String>>.broadcast();

  @override
  Future<void> initialize() async {}

  @override
  Future<bool> requestPermissions() async => false;

  @override
  Future<String?> getDeviceToken() async => null;

  @override
  Stream<Map<String, String>> get onNotificationOpened => _opened.stream;

  @override
  Stream<Map<String, String>> get onForegroundMessage => _foreground.stream;

  @override
  Future<void> dispose() async {
    await _opened.close();
    await _foreground.close();
  }
}
