import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/offline/offline_providers.dart';
import 'package:ashraak_mobile/core/sync/sync_providers.dart';
import 'package:ashraak_mobile/shared/utils/date_format.dart';

class OfflineBanner extends ConsumerWidget {
  const OfflineBanner({super.key, required this.child});

  final Widget child;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final status = ref.watch(offlineStatusProvider);

    return Column(
      children: [
        if (!status.isOnline)
          MaterialBanner(
            content: Text(
              status.lastSyncAt != null
                  ? 'Offline — last sync ${formatDateTime(status.lastSyncAt!)}'
                  : 'Offline — showing cached data where available',
            ),
            leading: const Icon(Icons.cloud_off_outlined),
            actions: [
              TextButton(
                onPressed: status.isSyncing
                    ? null
                    : () => ref.read(syncServiceProvider).syncAll(),
                child: Text(status.isSyncing ? 'Syncing…' : 'Retry'),
              ),
            ],
          ),
        Expanded(child: child),
      ],
    );
  }
}
