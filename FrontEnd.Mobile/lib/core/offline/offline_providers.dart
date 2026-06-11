import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/offline/connectivity_service.dart';
import 'package:ashraak_mobile/core/offline/hive_offline_cache.dart';
import 'package:ashraak_mobile/core/offline/offline_cache.dart';

final offlineCacheProvider = Provider<OfflineCache>((ref) => HiveOfflineCache());

class OfflineStatus {
  const OfflineStatus({
    this.isOnline = true,
    this.lastSyncAt,
    this.isSyncing = false,
  });

  final bool isOnline;
  final DateTime? lastSyncAt;
  final bool isSyncing;

  OfflineStatus copyWith({
    bool? isOnline,
    DateTime? lastSyncAt,
    bool? isSyncing,
  }) {
    return OfflineStatus(
      isOnline: isOnline ?? this.isOnline,
      lastSyncAt: lastSyncAt ?? this.lastSyncAt,
      isSyncing: isSyncing ?? this.isSyncing,
    );
  }
}

class OfflineStatusNotifier extends Notifier<OfflineStatus> {
  @override
  OfflineStatus build() {
    final connectivity = ref.read(connectivityServiceProvider);
    Future.microtask(() async {
      await connectivity.start();
      connectivity.onConnectivityChanged.listen((online) {
        state = state.copyWith(isOnline: online);
      });
      final online = await connectivity.isOnline();
      final cache = ref.read(offlineCacheProvider);
      final last = await cache.lastSyncedAt('global');
      state = OfflineStatus(isOnline: online, lastSyncAt: last);
    });
    return const OfflineStatus();
  }

  void setSyncing(bool syncing) => state = state.copyWith(isSyncing: syncing);

  Future<void> markSynced() async {
    final now = DateTime.now().toUtc();
    await ref.read(offlineCacheProvider).setLastSyncedAt('global', now);
    state = state.copyWith(lastSyncAt: now, isSyncing: false);
  }
}

final offlineStatusProvider =
    NotifierProvider<OfflineStatusNotifier, OfflineStatus>(OfflineStatusNotifier.new);
