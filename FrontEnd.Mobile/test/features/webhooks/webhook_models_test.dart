import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/features/webhooks/models/webhook_models.dart';

void main() {
  group('computeOverviewMetrics', () {
    test('calculates success and failure rates', () {
      final deliveries = [
        WebhookDelivery(
          id: '1',
          subscriptionId: 'sub',
          eventName: 'user.created',
          eventVersion: '1.0',
          correlationId: 'c1',
          attemptNumber: 1,
          retryCount: 0,
          status: 'Succeeded',
          startedOnUtc: DateTime.utc(2026, 1, 1),
        ),
        WebhookDelivery(
          id: '2',
          subscriptionId: 'sub',
          eventName: 'user.updated',
          eventVersion: '1.0',
          correlationId: 'c2',
          attemptNumber: 1,
          retryCount: 2,
          status: 'Failed',
          startedOnUtc: DateTime.utc(2026, 1, 2),
        ),
      ];

      final metrics = computeOverviewMetrics(
        deliveries: deliveries,
        deadLetters: const [],
      );

      expect(metrics.totalDeliveries, 2);
      expect(metrics.successRate, 50);
      expect(metrics.failureRate, 50);
      expect(metrics.retryCount, 2);
      expect(metrics.recentFailures.length, 1);
    });
  });

  group('applyDeliveryFilters', () {
    test('filters by status and search', () {
      final deliveries = [
        WebhookDelivery(
          id: 'abc-123',
          subscriptionId: 'sub',
          eventName: 'file.uploaded',
          eventVersion: '1.0',
          correlationId: 'corr-1',
          attemptNumber: 1,
          retryCount: 0,
          status: 'Succeeded',
        ),
        WebhookDelivery(
          id: 'def-456',
          subscriptionId: 'sub',
          eventName: 'file.deleted',
          eventVersion: '1.0',
          correlationId: 'corr-2',
          attemptNumber: 1,
          retryCount: 1,
          status: 'Failed',
        ),
      ];

      final filtered = applyDeliveryFilters(
        deliveries,
        const DeliveryFilters(status: 'Failed', search: 'corr-2'),
      );

      expect(filtered.length, 1);
      expect(filtered.first.id, 'def-456');
    });
  });

  group('applyDeadLetterFilters', () {
    test('filters by failure code', () {
      final items = [
        WebhookDeadLetter(
          id: 'dlq-1',
          deliveryId: 'd1',
          subscriptionId: 'sub',
          eventName: 'user.created',
          payload: '{}',
          failureReason: 'timeout',
          failureCode: 'Timeout',
          retryCount: 5,
          correlationId: 'c1',
          createdOnUtc: DateTime.utc(2026, 1, 1),
        ),
        WebhookDeadLetter(
          id: 'dlq-2',
          deliveryId: 'd2',
          subscriptionId: 'sub',
          eventName: 'user.updated',
          payload: '{}',
          failureReason: 'bad gateway',
          failureCode: 'HttpError',
          retryCount: 5,
          correlationId: 'c2',
          createdOnUtc: DateTime.utc(2026, 1, 2),
        ),
      ];

      final filtered = applyDeadLetterFilters(
        items,
        const DeadLetterFilters(failureCode: 'Timeout'),
      );

      expect(filtered.length, 1);
      expect(filtered.first.id, 'dlq-1');
    });
  });
}
