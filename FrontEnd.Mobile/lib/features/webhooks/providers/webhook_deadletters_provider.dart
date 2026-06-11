import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/offline/offline_cache_keys.dart';
import 'package:ashraak_mobile/core/offline/offline_providers.dart';
import 'package:ashraak_mobile/features/webhooks/data/webhooks_repository.dart';
import 'package:ashraak_mobile/features/webhooks/models/webhook_models.dart';

class WebhookDeadLettersState {
  const WebhookDeadLettersState({
    this.deadLetters = const [],
    this.filters = const DeadLetterFilters(),
    this.isLoading = false,
    this.isRefreshing = false,
    this.error,
    this.lastSyncAt,
  });

  final List<WebhookDeadLetter> deadLetters;
  final DeadLetterFilters filters;
  final bool isLoading;
  final bool isRefreshing;
  final Object? error;
  final DateTime? lastSyncAt;

  List<WebhookDeadLetter> get filtered =>
      applyDeadLetterFilters(deadLetters, filters);

  WebhookDeadLettersState copyWith({
    List<WebhookDeadLetter>? deadLetters,
    DeadLetterFilters? filters,
    bool? isLoading,
    bool? isRefreshing,
    Object? error,
    DateTime? lastSyncAt,
    bool clearError = false,
  }) {
    return WebhookDeadLettersState(
      deadLetters: deadLetters ?? this.deadLetters,
      filters: filters ?? this.filters,
      isLoading: isLoading ?? this.isLoading,
      isRefreshing: isRefreshing ?? this.isRefreshing,
      error: clearError ? null : (error ?? this.error),
      lastSyncAt: lastSyncAt ?? this.lastSyncAt,
    );
  }
}

class WebhookDeadLettersNotifier extends Notifier<WebhookDeadLettersState> {
  @override
  WebhookDeadLettersState build() {
    Future.microtask(load);
    return const WebhookDeadLettersState(isLoading: true);
  }

  WebhooksRepository get _repo => ref.read(webhooksRepositoryProvider);

  Future<void> load() async {
    state = state.copyWith(isLoading: true, clearError: true);
    try {
      final deadLetters = await _repo.listDeadLetters(limit: 200);
      final lastSync = await ref
          .read(offlineCacheProvider)
          .lastSyncedAt(OfflineCacheKeys.syncWebhooks);
      state = state.copyWith(
        deadLetters: deadLetters,
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
      final deadLetters = await _repo.listDeadLetters(limit: 200);
      final now = DateTime.now().toUtc();
      await ref
          .read(offlineCacheProvider)
          .setLastSyncedAt(OfflineCacheKeys.syncWebhooks, now);
      state = state.copyWith(
        deadLetters: deadLetters,
        isRefreshing: false,
        lastSyncAt: now,
      );
    } catch (e) {
      state = state.copyWith(isRefreshing: false, error: e);
    }
  }

  void setFilters(DeadLetterFilters filters) {
    state = state.copyWith(filters: filters);
  }
}

final webhookDeadLettersProvider =
    NotifierProvider<WebhookDeadLettersNotifier, WebhookDeadLettersState>(
  WebhookDeadLettersNotifier.new,
);

class WebhookDeadLetterDetailState {
  const WebhookDeadLetterDetailState({
    this.deadLetter,
    this.isLoading = false,
    this.error,
  });

  final WebhookDeadLetter? deadLetter;
  final bool isLoading;
  final Object? error;

  WebhookDeadLetterDetailState copyWith({
    WebhookDeadLetter? deadLetter,
    bool? isLoading,
    Object? error,
    bool clearError = false,
  }) {
    return WebhookDeadLetterDetailState(
      deadLetter: deadLetter ?? this.deadLetter,
      isLoading: isLoading ?? this.isLoading,
      error: clearError ? null : (error ?? this.error),
    );
  }
}

class WebhookDeadLetterDetailNotifier
    extends FamilyNotifier<WebhookDeadLetterDetailState, String> {
  @override
  WebhookDeadLetterDetailState build(String deadLetterId) {
    Future.microtask(() => load(deadLetterId));
    return const WebhookDeadLetterDetailState(isLoading: true);
  }

  WebhooksRepository get _repo => ref.read(webhooksRepositoryProvider);

  Future<void> load(String deadLetterId) async {
    state = state.copyWith(isLoading: true, clearError: true);
    try {
      final deadLetter = await _repo.getDeadLetter(deadLetterId);
      state = state.copyWith(deadLetter: deadLetter, isLoading: false);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e);
    }
  }
}

final webhookDeadLetterDetailProvider = NotifierProvider.family<
    WebhookDeadLetterDetailNotifier,
    WebhookDeadLetterDetailState,
    String>(WebhookDeadLetterDetailNotifier.new);
