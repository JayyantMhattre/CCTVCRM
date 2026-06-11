import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/offline/offline_cache_keys.dart';
import 'package:ashraak_mobile/core/offline/offline_providers.dart';
import 'package:ashraak_mobile/features/webhooks/data/webhooks_repository.dart';
import 'package:ashraak_mobile/features/webhooks/models/webhook_models.dart';

class WebhookDeliveriesState {
  const WebhookDeliveriesState({
    this.deliveries = const [],
    this.filters = const DeliveryFilters(),
    this.isLoading = false,
    this.isRefreshing = false,
    this.error,
    this.lastSyncAt,
  });

  final List<WebhookDelivery> deliveries;
  final DeliveryFilters filters;
  final bool isLoading;
  final bool isRefreshing;
  final Object? error;
  final DateTime? lastSyncAt;

  List<WebhookDelivery> get filtered =>
      applyDeliveryFilters(deliveries, filters);

  WebhookDeliveriesState copyWith({
    List<WebhookDelivery>? deliveries,
    DeliveryFilters? filters,
    bool? isLoading,
    bool? isRefreshing,
    Object? error,
    DateTime? lastSyncAt,
    bool clearError = false,
  }) {
    return WebhookDeliveriesState(
      deliveries: deliveries ?? this.deliveries,
      filters: filters ?? this.filters,
      isLoading: isLoading ?? this.isLoading,
      isRefreshing: isRefreshing ?? this.isRefreshing,
      error: clearError ? null : (error ?? this.error),
      lastSyncAt: lastSyncAt ?? this.lastSyncAt,
    );
  }
}

class WebhookDeliveriesNotifier extends Notifier<WebhookDeliveriesState> {
  @override
  WebhookDeliveriesState build() {
    Future.microtask(load);
    return const WebhookDeliveriesState(isLoading: true);
  }

  WebhooksRepository get _repo => ref.read(webhooksRepositoryProvider);

  Future<void> load() async {
    state = state.copyWith(isLoading: true, clearError: true);
    try {
      final deliveries = await _repo.listDeliveries(limit: 200);
      final lastSync = await ref
          .read(offlineCacheProvider)
          .lastSyncedAt(OfflineCacheKeys.syncWebhooks);
      state = state.copyWith(
        deliveries: deliveries,
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
      final deliveries = await _repo.listDeliveries(limit: 200);
      final now = DateTime.now().toUtc();
      await ref
          .read(offlineCacheProvider)
          .setLastSyncedAt(OfflineCacheKeys.syncWebhooks, now);
      state = state.copyWith(
        deliveries: deliveries,
        isRefreshing: false,
        lastSyncAt: now,
      );
    } catch (e) {
      state = state.copyWith(isRefreshing: false, error: e);
    }
  }

  void setFilters(DeliveryFilters filters) {
    state = state.copyWith(filters: filters);
  }
}

final webhookDeliveriesProvider =
    NotifierProvider<WebhookDeliveriesNotifier, WebhookDeliveriesState>(
  WebhookDeliveriesNotifier.new,
);

class WebhookDeliveryDetailState {
  const WebhookDeliveryDetailState({
    this.delivery,
    this.isLoading = false,
    this.error,
  });

  final WebhookDelivery? delivery;
  final bool isLoading;
  final Object? error;

  WebhookDeliveryDetailState copyWith({
    WebhookDelivery? delivery,
    bool? isLoading,
    Object? error,
    bool clearError = false,
  }) {
    return WebhookDeliveryDetailState(
      delivery: delivery ?? this.delivery,
      isLoading: isLoading ?? this.isLoading,
      error: clearError ? null : (error ?? this.error),
    );
  }
}

class WebhookDeliveryDetailNotifier extends FamilyNotifier<
    WebhookDeliveryDetailState, String> {
  @override
  WebhookDeliveryDetailState build(String deliveryId) {
    Future.microtask(() => load(deliveryId));
    return const WebhookDeliveryDetailState(isLoading: true);
  }

  WebhooksRepository get _repo => ref.read(webhooksRepositoryProvider);

  Future<void> load(String deliveryId) async {
    state = state.copyWith(isLoading: true, clearError: true);
    try {
      final delivery = await _repo.getDelivery(deliveryId);
      state = state.copyWith(delivery: delivery, isLoading: false);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e);
    }
  }
}

final webhookDeliveryDetailProvider = NotifierProvider.family<
    WebhookDeliveryDetailNotifier,
    WebhookDeliveryDetailState,
    String>(WebhookDeliveryDetailNotifier.new);
