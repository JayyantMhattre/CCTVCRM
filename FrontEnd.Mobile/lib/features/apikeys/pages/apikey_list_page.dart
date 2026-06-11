import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/apikeys/providers/apikey_list_provider.dart';
import 'package:ashraak_mobile/features/apikeys/widgets/apikey_status_badge.dart';
import 'package:ashraak_mobile/features/webhooks/widgets/last_sync_footer.dart';
import 'package:ashraak_mobile/shared/utils/date_format.dart';
import 'package:ashraak_mobile/shared/widgets/error_view.dart';
import 'package:ashraak_mobile/shared/widgets/loading_view.dart';

class ApiKeyListPage extends ConsumerWidget {
  const ApiKeyListPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(apiKeyListProvider);

    if (state.isLoading && state.apiKeys.isEmpty) {
      return const LoadingView(message: 'Loading API keys…');
    }

    if (state.error != null && state.apiKeys.isEmpty) {
      return ErrorView(
        error: state.error!,
        onRetry: () => ref.read(apiKeyListProvider.notifier).load(),
      );
    }

    final summary = state.summary;

    return RefreshIndicator(
      onRefresh: () => ref.read(apiKeyListProvider.notifier).refresh(),
      child: ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.all(16),
        children: [
          Text(
            'API keys',
            style: Theme.of(context).textTheme.headlineSmall,
          ),
          const SizedBox(height: 8),
          const Text('Read-only visibility — view key metadata and usage.'),
          const SizedBox(height: 16),
          _SummaryGrid(
            items: [
              _SummaryTile(label: 'Total keys', value: '${summary.total}'),
              _SummaryTile(label: 'Active', value: '${summary.active}'),
              _SummaryTile(label: 'Revoked', value: '${summary.revoked}'),
              _SummaryTile(label: 'Expired', value: '${summary.expired}'),
              _SummaryTile(
                label: 'Total requests',
                value: '${summary.totalRequests}',
              ),
            ],
          ),
          const SizedBox(height: 16),
          if (state.apiKeys.isEmpty)
            const Text('No API keys found for this tenant.')
          else
            ...state.apiKeys.map(
              (key) => Card(
                margin: const EdgeInsets.only(bottom: 8),
                child: ListTile(
                  title: Text(key.name),
                  subtitle: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text('${key.keyPrefix}… · ${key.environment}'),
                      if (key.lastUsedOnUtc != null)
                        Text('Last used ${formatDateTime(key.lastUsedOnUtc!)}'),
                    ],
                  ),
                  trailing: ApiKeyStatusBadge(status: key.status),
                  isThreeLine: key.lastUsedOnUtc != null,
                  onTap: () => context.go(RoutePaths.apiKeyDetail(key.id)),
                ),
              ),
            ),
          LastSyncFooter(lastSyncAt: state.lastSyncAt),
        ],
      ),
    );
  }
}

class _SummaryGrid extends StatelessWidget {
  const _SummaryGrid({required this.items});

  final List<_SummaryTile> items;

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

class _SummaryTile {
  const _SummaryTile({required this.label, required this.value});

  final String label;
  final String value;
}
