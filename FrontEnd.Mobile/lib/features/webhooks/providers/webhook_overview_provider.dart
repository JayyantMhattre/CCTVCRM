import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/offline/offline_cache_keys.dart';
import 'package:ashraak_mobile/core/offline/offline_providers.dart';
import 'package:ashraak_mobile/features/webhooks/data/webhooks_repository.dart';
import 'package:ashraak_mobile/features/webhooks/models/webhook_models.dart';

class WebhookOverviewState {
  const WebhookOverviewState({
    this.metrics,
    this.isLoading = false,
    this.isRefreshing = false,
    this.error,
    this.lastSyncAt,
  });

  final WebhookOverviewMetrics? metrics;
  final bool isLoading;
  final bool isRefreshing;
  final Object? error;
  final DateTime? lastSyncAt;

  WebhookOverviewState copyWith({
    WebhookOverviewMetrics? metrics,
    bool? isLoading,
    bool? isRefreshing,
    Object? error,
    DateTime? lastSyncAt,
    bool clearError = false,
  }) {
    return WebhookOverviewState(
      metrics: metrics ?? this.metrics,
      isLoading: isLoading ?? this.isLoading,
      isRefreshing: isRefreshing ?? this.isRefreshing,
      error: clearError ? null : (error ?? this.error),
      lastSyncAt: lastSyncAt ?? this.lastSyncAt,
    );
  }
}

class WebhookOverviewNotifier extends Notifier<WebhookOverviewState> {
  @override
  WebhookOverviewState build() {
    Future.microtask(load);
    return const WebhookOverviewState(isLoading: true);
  }

  WebhooksRepository get _repo => ref.read(webhooksRepositoryProvider);

  Future<void> load() async {
    state = state.copyWith(isLoading: true, clearError: true);
    try {
      final metrics = await _repo.getOverview();
      final lastSync = await ref
          .read(offlineCacheProvider)
          .lastSyncedAt(OfflineCacheKeys.syncWebhooks);
      state = state.copyWith(
        metrics: metrics,
        isLoading: false,
        lastSyncAt: lastSync,
      );
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e);
    }
  }

  Future<void> refresh() async {
    state = state.copyWith(isRefreshing: true, clearError: true);
    try {
      final metrics = await _repo.getOverview();
      final now = DateTime.now().toUtc();
      await ref
          .read(offlineCacheProvider)
          .setLastSyncedAt(OfflineCacheKeys.syncWebhooks, now);
      state = state.copyWith(
        metrics: metrics,
        isRefreshing: false,
        lastSyncAt: now,
      );
    } catch (e) {
      state = state.copyWith(isRefreshing: false, error: e);
    }
  }
}

final webhookOverviewProvider =
    NotifierProvider<WebhookOverviewNotifier, WebhookOverviewState>(
  WebhookOverviewNotifier.new,
);
