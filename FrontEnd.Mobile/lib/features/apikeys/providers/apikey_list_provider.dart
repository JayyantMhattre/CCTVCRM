import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/offline/offline_cache_keys.dart';
import 'package:ashraak_mobile/core/offline/offline_providers.dart';
import 'package:ashraak_mobile/features/apikeys/data/apikeys_repository.dart';
import 'package:ashraak_mobile/features/apikeys/models/apikey_models.dart';

class ApiKeyListState {
  const ApiKeyListState({
    this.apiKeys = const [],
    this.isLoading = false,
    this.isRefreshing = false,
    this.error,
    this.lastSyncAt,
  });

  final List<ApiKey> apiKeys;
  final bool isLoading;
  final bool isRefreshing;
  final Object? error;
  final DateTime? lastSyncAt;

  ApiKeyListSummary get summary => computeApiKeyListSummary(apiKeys);

  ApiKeyListState copyWith({
    List<ApiKey>? apiKeys,
    bool? isLoading,
    bool? isRefreshing,
    Object? error,
    DateTime? lastSyncAt,
    bool clearError = false,
  }) {
    return ApiKeyListState(
      apiKeys: apiKeys ?? this.apiKeys,
      isLoading: isLoading ?? this.isLoading,
      isRefreshing: isRefreshing ?? this.isRefreshing,
      error: clearError ? null : (error ?? this.error),
      lastSyncAt: lastSyncAt ?? this.lastSyncAt,
    );
  }
}

class ApiKeyListNotifier extends Notifier<ApiKeyListState> {
  @override
  ApiKeyListState build() {
    Future.microtask(load);
    return const ApiKeyListState(isLoading: true);
  }

  ApiKeysRepository get _repo => ref.read(apiKeysRepositoryProvider);

  Future<void> load() async {
    state = state.copyWith(isLoading: true, clearError: true);
    try {
      final apiKeys = await _repo.listApiKeys();
      final lastSync = await ref
          .read(offlineCacheProvider)
          .lastSyncedAt(OfflineCacheKeys.syncApiKeys);
      state = state.copyWith(
        apiKeys: apiKeys,
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
      final apiKeys = await _repo.listApiKeys();
      final now = DateTime.now().toUtc();
      await ref
          .read(offlineCacheProvider)
          .setLastSyncedAt(OfflineCacheKeys.syncApiKeys, now);
      state = state.copyWith(
        apiKeys: apiKeys,
        isRefreshing: false,
        lastSyncAt: now,
      );
    } catch (e) {
      state = state.copyWith(isRefreshing: false, error: e);
    }
  }
}

final apiKeyListProvider =
    NotifierProvider<ApiKeyListNotifier, ApiKeyListState>(
  ApiKeyListNotifier.new,
);
