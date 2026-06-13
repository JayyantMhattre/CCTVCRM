import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/offline/cctv_offline_queue.dart';
import 'package:ashraak_mobile/features/cctv_engineer/data/cctv_engineer_repository.dart';
import 'package:ashraak_mobile/shared/ui/app_toast.dart';
import 'package:ashraak_mobile/shared/widgets/empty_state.dart';

class CctvEngineerSyncPage extends ConsumerStatefulWidget {
  const CctvEngineerSyncPage({super.key});

  @override
  ConsumerState<CctvEngineerSyncPage> createState() => _CctvEngineerSyncPageState();
}

class _CctvEngineerSyncPageState extends ConsumerState<CctvEngineerSyncPage> {
  late Future<List<CctvOfflineQueueItem>> _future;
  bool _syncing = false;

  @override
  void initState() {
    super.initState();
    _reload();
  }

  void _reload() {
    _future = ref.read(cctvEngineerRepositoryProvider).listPendingQueue();
  }

  Future<void> _syncNow() async {
    setState(() => _syncing = true);
    try {
      final count = await ref.read(cctvEngineerRepositoryProvider).syncPendingQueue();
      if (mounted) AppToast.success(context, 'Synced $count pending item(s).');
      setState(_reload);
    } catch (error) {
      if (mounted) AppToast.error(context, error.toString());
    } finally {
      if (mounted) setState(() => _syncing = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<List<CctvOfflineQueueItem>>(
      future: _future,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        }
        if (snapshot.hasError) {
          return Center(child: Text(snapshot.error.toString()));
        }

        final items = snapshot.data ?? const [];
        return ListView(
          padding: const EdgeInsets.all(16),
          children: [
            Text('Offline sync', style: Theme.of(context).textTheme.headlineSmall),
            const SizedBox(height: 8),
            Text('Pending operations are replayed against existing visit and ticket APIs.'),
            const SizedBox(height: 16),
            FilledButton(
              onPressed: _syncing ? null : _syncNow,
              child: Text(_syncing ? 'Syncing…' : 'Retry sync now'),
            ),
            const SizedBox(height: 16),
            if (items.isEmpty)
              const EmptyState(
                title: 'Queue empty',
                description: 'All visit and ticket changes are synced.',
              )
            else
              ...items.map(
                (item) => Card(
                  child: ListTile(
                    title: Text(item.kind.name),
                    subtitle: Text(
                      [
                        if (item.visitId != null) 'Visit: ${item.visitId}',
                        'Attempts: ${item.attempts}',
                        if (item.lastError != null) item.lastError,
                      ].whereType<String>().join('\n'),
                    ),
                  ),
                ),
              ),
          ],
        );
      },
    );
  }
}
