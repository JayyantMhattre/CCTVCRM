import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/webhooks/providers/webhook_overview_provider.dart';
import 'package:ashraak_mobile/features/webhooks/widgets/last_sync_footer.dart';
import 'package:ashraak_mobile/features/webhooks/widgets/webhook_status_badge.dart';
import 'package:ashraak_mobile/shared/utils/date_format.dart';
import 'package:ashraak_mobile/shared/widgets/error_view.dart';
import 'package:ashraak_mobile/shared/widgets/loading_view.dart';

class WebhookOverviewPage extends ConsumerWidget {
  const WebhookOverviewPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(webhookOverviewProvider);

    if (state.isLoading && state.metrics == null) {
      return const LoadingView(message: 'Loading webhook dashboard…');
    }

    if (state.error != null && state.metrics == null) {
      return ErrorView(
        error: state.error!,
        onRetry: () => ref.read(webhookOverviewProvider.notifier).load(),
      );
    }

    final metrics = state.metrics!;

    return RefreshIndicator(
      onRefresh: () => ref.read(webhookOverviewProvider.notifier).refresh(),
      child: ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.all(16),
        children: [
          Text(
            'Webhook operations',
            style: Theme.of(context).textTheme.headlineSmall,
          ),
          const SizedBox(height: 8),
          const Text('Read-only visibility — monitor deliveries and dead letters.'),
          const SizedBox(height: 16),
          _MetricGrid(
            items: [
              _MetricTile(label: 'Total deliveries', value: '${metrics.totalDeliveries}'),
              _MetricTile(
                label: 'Success rate',
                value: '${metrics.successRate.toStringAsFixed(1)}%',
              ),
              _MetricTile(
                label: 'Failure rate',
                value: '${metrics.failureRate.toStringAsFixed(1)}%',
              ),
              _MetricTile(label: 'Retry count', value: '${metrics.retryCount}'),
              _MetricTile(label: 'Dead letters', value: '${metrics.deadLetterCount}'),
            ],
          ),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: FilledButton.tonal(
                  onPressed: () => context.go(RoutePaths.webhookDeliveries),
                  child: const Text('Delivery history'),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: FilledButton.tonal(
                  onPressed: () => context.go(RoutePaths.webhookDeadLetters),
                  child: const Text('Dead letters'),
                ),
              ),
            ],
          ),
          const SizedBox(height: 24),
          Text('Recent failures', style: Theme.of(context).textTheme.titleMedium),
          const SizedBox(height: 8),
          if (metrics.recentFailures.isEmpty)
            const Text('No recent failures in sample window.')
          else
            ...metrics.recentFailures.map(
              (d) => _DeliveryTile(
                eventName: d.eventName,
                status: d.status,
                timestamp: d.startedOnUtc,
                onTap: () => context.go(RoutePaths.webhookDeliveryDetail(d.id)),
              ),
            ),
          const SizedBox(height: 24),
          Text('Recent deliveries', style: Theme.of(context).textTheme.titleMedium),
          const SizedBox(height: 8),
          if (metrics.recentDeliveries.isEmpty)
            const Text('No recent deliveries in sample window.')
          else
            ...metrics.recentDeliveries.map(
              (d) => _DeliveryTile(
                eventName: d.eventName,
                status: d.status,
                timestamp: d.startedOnUtc,
                onTap: () => context.go(RoutePaths.webhookDeliveryDetail(d.id)),
              ),
            ),
          LastSyncFooter(lastSyncAt: state.lastSyncAt),
        ],
      ),
    );
  }
}

class _MetricGrid extends StatelessWidget {
  const _MetricGrid({required this.items});

  final List<_MetricTile> items;

  @override
  Widget build(BuildContext context) {
    return Wrap(
      spacing: 12,
      runSpacing: 12,
      children: items
          .map(
            (item) => SizedBox(
              width: (MediaQuery.sizeOf(context).width - 44) / 2,
              child: Card(
                child: Padding(
                  padding: const EdgeInsets.all(12),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(item.label, style: Theme.of(context).textTheme.bodySmall),
                      const SizedBox(height: 4),
                      Text(
                        item.value,
                        style: Theme.of(context).textTheme.titleLarge,
                      ),
                    ],
                  ),
                ),
              ),
            ),
          )
          .toList(),
    );
  }
}

class _MetricTile {
  const _MetricTile({required this.label, required this.value});

  final String label;
  final String value;
}

class _DeliveryTile extends StatelessWidget {
  const _DeliveryTile({
    required this.eventName,
    required this.status,
    required this.timestamp,
    required this.onTap,
  });

  final String eventName;
  final String status;
  final DateTime? timestamp;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        title: Text(eventName),
        subtitle: timestamp != null ? Text(formatDateTime(timestamp!)) : null,
        trailing: WebhookStatusBadge(status: status),
        onTap: onTap,
      ),
    );
  }
}
