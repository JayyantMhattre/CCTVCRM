import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/webhooks/providers/webhook_deliveries_provider.dart';
import 'package:ashraak_mobile/features/webhooks/widgets/webhook_status_badge.dart';
import 'package:ashraak_mobile/shared/utils/date_format.dart';
import 'package:ashraak_mobile/shared/widgets/correlation_banner.dart';
import 'package:ashraak_mobile/shared/widgets/error_view.dart';
import 'package:ashraak_mobile/shared/widgets/loading_view.dart';

class WebhookDeliveryDetailPage extends ConsumerWidget {
  const WebhookDeliveryDetailPage({super.key, required this.deliveryId});

  final String deliveryId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(webhookDeliveryDetailProvider(deliveryId));

    if (state.isLoading && state.delivery == null) {
      return const LoadingView(message: 'Loading delivery…');
    }

    if (state.error != null && state.delivery == null) {
      return ErrorView(
        error: state.error!,
        onRetry: () =>
            ref.read(webhookDeliveryDetailProvider(deliveryId).notifier).load(deliveryId),
      );
    }

    final delivery = state.delivery!;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Row(
          children: [
            Expanded(
              child: Text(
                delivery.eventName,
                style: Theme.of(context).textTheme.headlineSmall,
              ),
            ),
            WebhookStatusBadge(status: delivery.status),
          ],
        ),
        const SizedBox(height: 12),
        if (delivery.correlationId.isNotEmpty)
          CorrelationBanner(correlationId: delivery.correlationId),
        const SizedBox(height: 16),
        _Section(
          title: 'Summary',
          children: [
            _DetailRow('Delivery ID', delivery.id),
            _DetailRow('Subscription', delivery.subscriptionId),
            _DetailRow('Event version', delivery.eventVersion),
            _DetailRow('Attempt', '${delivery.attemptNumber}'),
            _DetailRow('Retry count', '${delivery.retryCount}'),
            if (delivery.responseCode != null)
              _DetailRow('Response code', '${delivery.responseCode}'),
            if (delivery.lastFailureCode != null)
              _DetailRow('Failure code', delivery.lastFailureCode!),
            if (delivery.lastFailureReason != null)
              _DetailRow('Failure reason', delivery.lastFailureReason!),
          ],
        ),
        _Section(
          title: 'Timeline',
          children: [
            if (delivery.startedOnUtc != null)
              _DetailRow('Started', formatDateTime(delivery.startedOnUtc!)),
            if (delivery.completedOnUtc != null)
              _DetailRow('Completed', formatDateTime(delivery.completedOnUtc!)),
            if (delivery.nextRetryOnUtc != null)
              _DetailRow('Next retry', formatDateTime(delivery.nextRetryOnUtc!)),
            if (delivery.duration != null)
              _DetailRow('Duration', '${delivery.duration!.inMilliseconds} ms'),
          ],
        ),
        _Section(
          title: 'Response summary',
          children: [
            SelectableText(
              delivery.responseBody?.isNotEmpty == true
                  ? delivery.responseBody!
                  : 'No response body recorded.',
              style: Theme.of(context).textTheme.bodyMedium,
            ),
          ],
        ),
        const SizedBox(height: 8),
        Text(
          'Read-only view — use the web Operations Center for retry or replay.',
          style: Theme.of(context).textTheme.bodySmall,
        ),
      ],
    );
  }
}

class _Section extends StatelessWidget {
  const _Section({required this.title, required this.children});

  final String title;
  final List<Widget> children;

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(title, style: Theme.of(context).textTheme.titleMedium),
            const SizedBox(height: 8),
            ...children,
          ],
        ),
      ),
    );
  }
}

class _DetailRow extends StatelessWidget {
  const _DetailRow(this.label, this.value);

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 6),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 120,
            child: Text(label, style: Theme.of(context).textTheme.bodySmall),
          ),
          Expanded(child: SelectableText(value)),
        ],
      ),
    );
  }
}
