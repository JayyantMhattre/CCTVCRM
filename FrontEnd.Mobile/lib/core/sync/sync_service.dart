import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/auth/current_user.dart';
import 'package:ashraak_mobile/core/offline/connectivity_service.dart';
import 'package:ashraak_mobile/core/offline/offline_cache_keys.dart';
import 'package:ashraak_mobile/core/offline/offline_providers.dart';
import 'package:ashraak_mobile/core/sync/sync_targets.dart';
import 'package:ashraak_mobile/features/files/providers/files_provider.dart';
import 'package:ashraak_mobile/features/notifications/providers/notifications_provider.dart';
import 'package:ashraak_mobile/features/profile/providers/profile_provider.dart';
import 'package:ashraak_mobile/features/settings/providers/settings_provider.dart';
import 'package:ashraak_mobile/features/apikeys/providers/apikey_list_provider.dart';
import 'package:ashraak_mobile/features/webhooks/providers/webhook_overview_provider.dart';

/// Battery-friendly refresh on connectivity resume — not polling.
class SyncService {
  SyncService(this._ref);

  final Ref _ref;

  Future<void> syncAll() async {
    final online = await _ref.read(connectivityServiceProvider).isOnline();
    if (!online) return;

    final user = _ref.read(currentUserProvider);
    if (user == null) return;

    _ref.read(offlineStatusProvider.notifier).setSyncing(true);

    try {
      await _syncTarget(SyncTarget.profile, () async {
        await _ref.read(profileProvider.notifier).load(user);
        await _ref.read(offlineCacheProvider).setLastSyncedAt(
              OfflineCacheKeys.syncProfile,
              DateTime.now().toUtc(),
            );
      });

      await _syncTarget(SyncTarget.settings, () async {
        await _ref.read(settingsProvider.notifier).load();
        await _ref.read(offlineCacheProvider).setLastSyncedAt(
              OfflineCacheKeys.syncSettings,
              DateTime.now().toUtc(),
            );
      });

      await _syncTarget(SyncTarget.notifications, () async {
        await _ref.read(notificationsProvider.notifier).load(user.userId);
        await _ref.read(offlineCacheProvider).setLastSyncedAt(
              OfflineCacheKeys.syncNotifications,
              DateTime.now().toUtc(),
            );
      });

      // Files metadata: session list already in memory; stamp sync time.
      await _ref.read(offlineCacheProvider).setLastSyncedAt(
            OfflineCacheKeys.syncFiles,
            DateTime.now().toUtc(),
          );

      if (user.canReadWebhooks) {
        await _syncTarget(SyncTarget.webhooks, () async {
          await _ref.read(webhookOverviewProvider.notifier).refresh();
          await _ref.read(offlineCacheProvider).setLastSyncedAt(
                OfflineCacheKeys.syncWebhooks,
                DateTime.now().toUtc(),
              );
        });
      }

      if (user.canReadApiKeys) {
        await _syncTarget(SyncTarget.apiKeys, () async {
          await _ref.read(apiKeyListProvider.notifier).refresh();
          await _ref.read(offlineCacheProvider).setLastSyncedAt(
                OfflineCacheKeys.syncApiKeys,
                DateTime.now().toUtc(),
              );
        });
      }

      await _ref.read(offlineStatusProvider.notifier).markSynced();
    } catch (_) {
      _ref.read(offlineStatusProvider.notifier).setSyncing(false);
    }
  }

  Future<void> _syncTarget(SyncTarget target, Future<void> Function() action) async {
    try {
      await action();
    } catch (_) {
      // Keep partial sync — online-first with cached fallback.
    }
  }
}
