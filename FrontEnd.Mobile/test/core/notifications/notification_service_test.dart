import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/core/notifications/flutter_local_notification_provider.dart';
import 'package:ashraak_mobile/core/notifications/noop_push_provider.dart';
import 'package:ashraak_mobile/core/notifications/notification_service.dart';

void main() {
  test('NotificationService initializes with no-op push provider', () async {
    final service = NotificationService(
      pushProvider: NoOpPushNotificationProvider(),
      localProvider: FlutterLocalNotificationProvider(),
    );

    await service.initialize();
    expect(await service.registerDeviceToken(), isNull);
    await service.dispose();
  });
}
