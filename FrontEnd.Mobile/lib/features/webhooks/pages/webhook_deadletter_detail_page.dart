import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/webhooks/providers/webhook_deadletters_provider.dart';
import 'package:ashraak_mobile/shared/utils/date_format.dart';
import 'package:ashraak_mobile/shared/widgets/correlation_banner.dart';
import 'package:ashraak_mobile/shared/widgets/error_view.dart';
import 'package:ashraak_mobile/shared/widgets/loading_view.dart';

class WebhookDeadLetterDetailPage extends ConsumerWidget {
  const WebhookDeadLetterDetailPage({super.key, required this.deadLetterId});

  final String deadLetterId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(webhookDeadLetterDetailProvider(deadLetterId));

    if (state.isLoading && state.deadLetter == null) {
      return const LoadingView(message: 'Loading dead letter…');
    }

    if (state.error != null && state.deadLetter == null) {
      return ErrorView(
        error: state.error!,
        onRetry: () => ref
            .read(webhookDeadLetterDetailProvider(deadLetterId).notifier)
            .load(deadLetterId),
      );
    }

    final item = state.deadLetter!;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text(item.eventName, style: Theme.of(context).textTheme.headlineSmall),
        const SizedBox(height: 12),
        if (item.correlationId.isNotEmpty)
          CorrelationBanner(correlationId: item.correlationId),
        const SizedBox(height: 16),
        _Section(
          title: 'Failure details',
          children: [
            _DetailRow('Dead letter ID', item.id),
            _DetailRow('Delivery ID', item.deliveryId),
            _DetailRow('Subscription', item.subscriptionId),
            _DetailRow('Failure code', item.failureCode),
            _DetailRow('Failure reason', item.failureReason),
            _DetailRow('Retry count', '${item.retryCount}'),
            _DetailRow('Created', formatDateTime(item.createdOnUtc)),
          ],
        ),
        _Section(
          title: 'Payload',
          children: [
            SelectableText(
              item.payload.isNotEmpty ? item.payload : 'No payload recorded.',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    fontFamily: 'monospace',
                  ),
            ),
          ],
        ),
        _Section(
          title: 'Retry history',
          children: [
            Text(
              'Retry count at dead-letter: ${item.retryCount}. '
              'Full retry timeline is available in the web Operations Center.',
              style: Theme.of(context).textTheme.bodyMedium,
            ),
          ],
        ),
        Text(
          'Read-only view — replay and recovery actions require the web admin console.',
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
