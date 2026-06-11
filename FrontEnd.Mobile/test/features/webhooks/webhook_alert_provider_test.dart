import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/core/notifications/notification_provider.dart';
import 'package:ashraak_mobile/features/webhooks/notifications/webhook_alert_provider.dart';

class _FakeLocalNotificationProvider implements LocalNotificationProvider {
  final shown = <Map<String, dynamic>>[];

  @override
  Future<void> cancelAll() async {}

  @override
  Future<void> initialize() async {}

  @override
  Future<void> show({
    required int id,
    required String title,
    required String body,
    Map<String, String>? payload,
  }) async {
    shown.add({'id': id, 'title': title, 'body': body, 'payload': payload});
  }
}

void main() {
  test('LocalWebhookAlertProvider shows delivery failure alert', () async {
    final local = _FakeLocalNotificationProvider();
    final provider = LocalWebhookAlertProvider(local);

    await provider.showDeliveryFailureAlert(
      eventName: 'user.created',
      correlationId: 'corr-1',
      deliveryId: 'del-1',
    );

    expect(local.shown.length, 1);
    expect(local.shown.first['title'], 'Webhook delivery failed');
    expect(local.shown.first['payload'], isNotNull);
  });
}
