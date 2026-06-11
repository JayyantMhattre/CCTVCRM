import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/webhooks/models/webhook_models.dart';
import 'package:ashraak_mobile/features/webhooks/providers/webhook_deliveries_provider.dart';
import 'package:ashraak_mobile/features/webhooks/widgets/last_sync_footer.dart';
import 'package:ashraak_mobile/features/webhooks/widgets/webhook_status_badge.dart';
import 'package:ashraak_mobile/shared/utils/date_format.dart';
import 'package:ashraak_mobile/shared/widgets/empty_state.dart';
import 'package:ashraak_mobile/shared/widgets/error_view.dart';
import 'package:ashraak_mobile/shared/widgets/loading_view.dart';

class WebhookDeliveriesPage extends ConsumerStatefulWidget {
  const WebhookDeliveriesPage({super.key});

  @override
  ConsumerState<WebhookDeliveriesPage> createState() =>
      _WebhookDeliveriesPageState();
}

class _WebhookDeliveriesPageState extends ConsumerState<WebhookDeliveriesPage> {
  final _searchController = TextEditingController();

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(webhookDeliveriesProvider);

    if (state.isLoading && state.deliveries.isEmpty) {
      return const LoadingView(message: 'Loading deliveries…');
    }

    if (state.error != null && state.deliveries.isEmpty) {
      return ErrorView(
        error: state.error!,
        onRetry: () => ref.read(webhookDeliveriesProvider.notifier).load(),
      );
    }

    final items = state.filtered;

    return Column(
      children: [
        _DeliveryFiltersBar(
          filters: state.filters,
          searchController: _searchController,
          onFiltersChanged: (filters) =>
              ref.read(webhookDeliveriesProvider.notifier).setFilters(filters),
        ),
        Expanded(
          child: RefreshIndicator(
            onRefresh: () =>
                ref.read(webhookDeliveriesProvider.notifier).refresh(),
            child: items.isEmpty
                ? ListView(
                    physics: const AlwaysScrollableScrollPhysics(),
                    children: const [
                      SizedBox(height: 80),
                      EmptyState(
                        title: 'No deliveries',
                        description: 'Adjust filters or refresh when online.',
                        icon: Icons.send_outlined,
                      ),
                    ],
                  )
                : ListView.separated(
                    physics: const AlwaysScrollableScrollPhysics(),
                    padding: const EdgeInsets.symmetric(horizontal: 16),
                    itemCount: items.length + 1,
                    separatorBuilder: (_, __) => const SizedBox(height: 8),
                    itemBuilder: (context, index) {
                      if (index == items.length) {
                        return LastSyncFooter(lastSyncAt: state.lastSyncAt);
                      }
                      final delivery = items[index];
                      return _DeliveryCard(
                        delivery: delivery,
                        onTap: () => context.go(
                          RoutePaths.webhookDeliveryDetail(delivery.id),
                        ),
                      );
                    },
                  ),
          ),
        ),
      ],
    );
  }
}

class _DeliveryFiltersBar extends StatelessWidget {
  const _DeliveryFiltersBar({
    required this.filters,
    required this.searchController,
    required this.onFiltersChanged,
  });

  final DeliveryFilters filters;
  final TextEditingController searchController;
  final ValueChanged<DeliveryFilters> onFiltersChanged;

  static const _statuses = ['', 'Succeeded', 'Failed', 'Retrying', 'DeadLettered'];

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          TextField(
            controller: searchController,
            decoration: const InputDecoration(
              labelText: 'Search',
              hintText: 'Event, correlation ID, delivery ID',
              prefixIcon: Icon(Icons.search),
            ),
            onChanged: (value) =>
                onFiltersChanged(filters.copyWith(search: value)),
          ),
          const SizedBox(height: 8),
          TextField(
            decoration: const InputDecoration(
              labelText: 'Event type',
              prefixIcon: Icon(Icons.event_outlined),
            ),
            onChanged: (value) =>
                onFiltersChanged(filters.copyWith(eventName: value)),
          ),
          const SizedBox(height: 8),
          DropdownButtonFormField<String>(
            value: filters.status ?? '',
            decoration: const InputDecoration(labelText: 'Status'),
            items: _statuses
                .map(
                  (s) => DropdownMenuItem(
                    value: s,
                    child: Text(s.isEmpty ? 'All statuses' : s),
                  ),
                )
                .toList(),
            onChanged: (value) => onFiltersChanged(
              filters.copyWith(
                status: value == null || value.isEmpty ? null : value,
                clearStatus: value == null || value.isEmpty,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _DeliveryCard extends StatelessWidget {
  const _DeliveryCard({required this.delivery, required this.onTap});

  final WebhookDelivery delivery;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final duration = delivery.duration;
    return Card(
      child: ListTile(
        title: Text(delivery.eventName),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (delivery.startedOnUtc != null)
              Text(formatDateTime(delivery.startedOnUtc!)),
            if (duration != null) Text('Duration: ${duration.inMilliseconds} ms'),
            if (delivery.responseCode != null)
              Text('Response: ${delivery.responseCode}'),
            if (delivery.correlationId.isNotEmpty)
              Text('Ref: ${delivery.correlationId}', maxLines: 1, overflow: TextOverflow.ellipsis),
          ],
        ),
        trailing: WebhookStatusBadge(status: delivery.status),
        isThreeLine: true,
        onTap: onTap,
      ),
    );
  }
}
