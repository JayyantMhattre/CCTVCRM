import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/webhooks/models/webhook_models.dart';
import 'package:ashraak_mobile/features/webhooks/providers/webhook_deadletters_provider.dart';
import 'package:ashraak_mobile/features/webhooks/widgets/last_sync_footer.dart';
import 'package:ashraak_mobile/shared/utils/date_format.dart';
import 'package:ashraak_mobile/shared/widgets/empty_state.dart';
import 'package:ashraak_mobile/shared/widgets/error_view.dart';
import 'package:ashraak_mobile/shared/widgets/loading_view.dart';

class WebhookDeadLettersPage extends ConsumerStatefulWidget {
  const WebhookDeadLettersPage({super.key});

  @override
  ConsumerState<WebhookDeadLettersPage> createState() =>
      _WebhookDeadLettersPageState();
}

class _WebhookDeadLettersPageState extends ConsumerState<WebhookDeadLettersPage> {
  final _searchController = TextEditingController();

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(webhookDeadLettersProvider);

    if (state.isLoading && state.deadLetters.isEmpty) {
      return const LoadingView(message: 'Loading dead letters…');
    }

    if (state.error != null && state.deadLetters.isEmpty) {
      return ErrorView(
        error: state.error!,
        onRetry: () => ref.read(webhookDeadLettersProvider.notifier).load(),
      );
    }

    final items = state.filtered;

    return Column(
      children: [
        _DeadLetterFiltersBar(
          filters: state.filters,
          searchController: _searchController,
          failureCodes: _distinctFailureCodes(state.deadLetters),
          onFiltersChanged: (filters) =>
              ref.read(webhookDeadLettersProvider.notifier).setFilters(filters),
        ),
        Expanded(
          child: RefreshIndicator(
            onRefresh: () =>
                ref.read(webhookDeadLettersProvider.notifier).refresh(),
            child: items.isEmpty
                ? ListView(
                    physics: const AlwaysScrollableScrollPhysics(),
                    children: const [
                      SizedBox(height: 80),
                      EmptyState(
                        title: 'No dead letters',
                        description: 'Failed deliveries appear here after retries exhaust.',
                        icon: Icons.inbox_outlined,
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
                      final item = items[index];
                      return _DeadLetterCard(
                        deadLetter: item,
                        onTap: () => context.go(
                          RoutePaths.webhookDeadLetterDetail(item.id),
                        ),
                      );
                    },
                  ),
          ),
        ),
      ],
    );
  }

  List<String> _distinctFailureCodes(List<WebhookDeadLetter> items) {
    return items.map((d) => d.failureCode).where((c) => c.isNotEmpty).toSet().toList()
      ..sort();
  }
}

class _DeadLetterFiltersBar extends StatelessWidget {
  const _DeadLetterFiltersBar({
    required this.filters,
    required this.searchController,
    required this.failureCodes,
    required this.onFiltersChanged,
  });

  final DeadLetterFilters filters;
  final TextEditingController searchController;
  final List<String> failureCodes;
  final ValueChanged<DeadLetterFilters> onFiltersChanged;

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
              hintText: 'Event, correlation ID, reason',
              prefixIcon: Icon(Icons.search),
            ),
            onChanged: (value) =>
                onFiltersChanged(filters.copyWith(search: value)),
          ),
          const SizedBox(height: 8),
          DropdownButtonFormField<String>(
            value: filters.failureCode ?? '',
            decoration: const InputDecoration(labelText: 'Failure type'),
            items: [
              const DropdownMenuItem(value: '', child: Text('All failure types')),
              ...failureCodes.map(
                (code) => DropdownMenuItem(value: code, child: Text(code)),
              ),
            ],
            onChanged: (value) => onFiltersChanged(
              filters.copyWith(
                failureCode: value == null || value.isEmpty ? null : value,
                clearFailureCode: value == null || value.isEmpty,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _DeadLetterCard extends StatelessWidget {
  const _DeadLetterCard({required this.deadLetter, required this.onTap});

  final WebhookDeadLetter deadLetter;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return Card(
      child: ListTile(
        title: Text(deadLetter.eventName),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(formatDateTime(deadLetter.createdOnUtc)),
            Text('Failure: ${deadLetter.failureReason}', maxLines: 2, overflow: TextOverflow.ellipsis),
            Text('Type: ${deadLetter.failureCode}'),
            Text('Retries: ${deadLetter.retryCount}'),
          ],
        ),
        isThreeLine: true,
        onTap: onTap,
      ),
    );
  }
}
