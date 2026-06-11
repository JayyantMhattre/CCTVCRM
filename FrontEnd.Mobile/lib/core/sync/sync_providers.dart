import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/offline/connectivity_service.dart';
import 'package:ashraak_mobile/core/sync/sync_service.dart';

final syncServiceProvider = Provider<SyncService>((ref) => SyncService(ref));

/// Triggers sync when app resumes and when connectivity returns.
class BackgroundSyncCoordinator extends WidgetsBindingObserver {
  BackgroundSyncCoordinator(this._ref);

  final Ref _ref;

  void start() {
    WidgetsBinding.instance.addObserver(this);
    _ref.read(connectivityServiceProvider).onConnectivityChanged.listen((online) {
      if (online) _ref.read(syncServiceProvider).syncAll();
    });
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (state == AppLifecycleState.resumed) {
      _ref.read(syncServiceProvider).syncAll();
    }
  }

  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
  }
}

final backgroundSyncCoordinatorProvider = Provider<BackgroundSyncCoordinator>((ref) {
  final coordinator = BackgroundSyncCoordinator(ref);
  coordinator.start();
  ref.onDispose(coordinator.dispose);
  return coordinator;
});
