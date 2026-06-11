import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/notifications/notification_provider.dart';
import 'package:ashraak_mobile/core/notifications/notification_providers.dart';

/// Future webhook operational alerts — local notifications only (W5).
abstract class WebhookAlertProvider {
  Future<void> showDeliveryFailureAlert({
    required String eventName,
    required String correlationId,
    String? deliveryId,
  });

  Future<void> showDeadLetterAlert({
    required String eventName,
    required String correlationId,
    String? deadLetterId,
  });

  Future<void> showRetryStormAlert({required int failureCount});

  Future<void> showSubscriptionDisabledAlert({required String subscriptionId});
}

class LocalWebhookAlertProvider implements WebhookAlertProvider {
  LocalWebhookAlertProvider(this._local);

  final LocalNotificationProvider _local;

  @override
  Future<void> showDeliveryFailureAlert({
    required String eventName,
    required String correlationId,
    String? deliveryId,
  }) async {
    await _local.show(
      id: correlationId.hashCode,
      title: 'Webhook delivery failed',
      body: '$eventName — ref $correlationId',
      payload: {
        if (deliveryId != null) 'deliveryId': deliveryId,
        'correlationId': correlationId,
        'type': 'webhook_failure',
      },
    );
  }

  @override
  Future<void> showDeadLetterAlert({
    required String eventName,
    required String correlationId,
    String? deadLetterId,
  }) async {
    await _local.show(
      id: correlationId.hashCode + 1,
      title: 'Webhook dead letter',
      body: '$eventName moved to DLQ',
      payload: {
        if (deadLetterId != null) 'deadLetterId': deadLetterId,
        'correlationId': correlationId,
        'type': 'webhook_dlq',
      },
    );
  }

  @override
  Future<void> showRetryStormAlert({required int failureCount}) async {
    await _local.show(
      id: 'retry_storm'.hashCode,
      title: 'Webhook retry storm',
      body: '$failureCount recent failures detected',
      payload: {'type': 'webhook_retry_storm'},
    );
  }

  @override
  Future<void> showSubscriptionDisabledAlert({
    required String subscriptionId,
  }) async {
    await _local.show(
      id: subscriptionId.hashCode,
      title: 'Webhook subscription disabled',
      body: 'Subscription $subscriptionId is disabled',
      payload: {
        'subscriptionId': subscriptionId,
        'type': 'webhook_subscription_disabled',
      },
    );
  }
}

final webhookAlertProviderProvider = Provider<WebhookAlertProvider>((ref) {
  return LocalWebhookAlertProvider(
    ref.watch(localNotificationProviderProvider),
  );
});
