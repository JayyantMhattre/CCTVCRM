import 'package:flutter_local_notifications/flutter_local_notifications.dart';

import 'package:ashraak_mobile/core/notifications/notification_provider.dart';

class FlutterLocalNotificationProvider implements LocalNotificationProvider {
  FlutterLocalNotificationProvider({FlutterLocalNotificationsPlugin? plugin})
      : _plugin = plugin ?? FlutterLocalNotificationsPlugin();

  final FlutterLocalNotificationsPlugin _plugin;

  @override
  Future<void> initialize() async {
    const android = AndroidInitializationSettings('@mipmap/ic_launcher');
    const ios = DarwinInitializationSettings();
    await _plugin.initialize(
      const InitializationSettings(android: android, iOS: ios),
    );
  }

  @override
  Future<void> show({
    required int id,
    required String title,
    required String body,
    Map<String, String>? payload,
  }) async {
    const details = NotificationDetails(
      android: AndroidNotificationDetails(
        'ashraak_default',
        'Ashraak',
        channelDescription: 'General notifications',
        importance: Importance.high,
        priority: Priority.high,
      ),
      iOS: DarwinNotificationDetails(),
    );

    await _plugin.show(
      id,
      title,
      body,
      details,
      payload: payload?.entries.map((e) => '${e.key}=${e.value}').join('&'),
    );
  }

  @override
  Future<void> cancelAll() => _plugin.cancelAll();
}
