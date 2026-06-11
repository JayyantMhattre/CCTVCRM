import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/api/base_api_client.dart';
import 'package:ashraak_mobile/core/offline/cached_repository_mixin.dart';
import 'package:ashraak_mobile/core/offline/offline_cache.dart';
import 'package:ashraak_mobile/core/offline/offline_cache_keys.dart';
import 'package:ashraak_mobile/core/offline/offline_providers.dart';
import 'package:ashraak_mobile/features/webhooks/models/webhook_models.dart';
import 'package:ashraak_mobile/shared/errors/api_error.dart';

class WebhooksRepository with CachedRepositoryMixin {
  WebhooksRepository(this._client, this._cache);

  final BaseApiClient _client;
  final OfflineCache _cache;

  static const _deliveriesPath = '/webhooks/deliveries';
  static const _deadLettersPath = '/webhooks/deadletters';

  Future<List<WebhookDelivery>> listDeliveries({
    String? subscriptionId,
    String? eventName,
    String? status,
    DateTime? fromUtc,
    DateTime? toUtc,
    int limit = 100,
  }) async {
    final query = <String, dynamic>{
      if (subscriptionId != null) 'subscriptionId': subscriptionId,
      if (eventName != null && eventName.isNotEmpty) 'eventName': eventName,
      if (status != null && status.isNotEmpty) 'status': status,
      if (fromUtc != null) 'fromUtc': fromUtc.toUtc().toIso8601String(),
      if (toUtc != null) 'toUtc': toUtc.toUtc().toIso8601String(),
      'limit': limit,
    };

    try {
      final response = await _client.get<List<dynamic>>(
        _deliveriesPath,
        queryParameters: query,
      );
      final items = parseDeliveriesList(response.data);
      await writeCached(
        cache: _cache,
        key: OfflineCacheKeys.webhookDeliveries,
        json: {'items': items.map((e) => e.toJson()).toList()},
      );
      return items;
    } on DioException catch (e) {
      final cached = await readCachedList<WebhookDelivery>(
        cache: _cache,
        key: OfflineCacheKeys.webhookDeliveries,
        fromJson: WebhookDelivery.fromJson,
      );
      if (cached != null) return cached;
      throw ApiError.fromDioException(e);
    }
  }

  Future<WebhookDelivery> getDelivery(String id) async {
    try {
      final response = await _client.get<Map<String, dynamic>>(
        '$_deliveriesPath/$id',
      );
      final data = response.data!;
      await writeCached(
        cache: _cache,
        key: OfflineCacheKeys.webhookDelivery(id),
        json: data,
      );
      return WebhookDelivery.fromJson(data);
    } on DioException catch (e) {
      final cached = await readCached<WebhookDelivery>(
        cache: _cache,
        key: OfflineCacheKeys.webhookDelivery(id),
        fromJson: WebhookDelivery.fromJson,
      );
      if (cached != null) return cached;
      throw ApiError.fromDioException(e);
    }
  }

  Future<List<WebhookDeadLetter>> listDeadLetters({
    String? subscriptionId,
    String? eventName,
    DateTime? fromUtc,
    DateTime? toUtc,
    int limit = 100,
  }) async {
    final query = <String, dynamic>{
      if (subscriptionId != null) 'subscriptionId': subscriptionId,
      if (eventName != null && eventName.isNotEmpty) 'eventName': eventName,
      if (fromUtc != null) 'fromUtc': fromUtc.toUtc().toIso8601String(),
      if (toUtc != null) 'toUtc': toUtc.toUtc().toIso8601String(),
      'limit': limit,
    };

    try {
      final response = await _client.get<List<dynamic>>(
        _deadLettersPath,
        queryParameters: query,
      );
      final items = parseDeadLettersList(response.data);
      await writeCached(
        cache: _cache,
        key: OfflineCacheKeys.webhookDeadLetters,
        json: {'items': items.map((e) => e.toJson()).toList()},
      );
      return items;
    } on DioException catch (e) {
      final cached = await readCachedList<WebhookDeadLetter>(
        cache: _cache,
        key: OfflineCacheKeys.webhookDeadLetters,
        fromJson: WebhookDeadLetter.fromJson,
      );
      if (cached != null) return cached;
      throw ApiError.fromDioException(e);
    }
  }

  Future<WebhookDeadLetter> getDeadLetter(String id) async {
    try {
      final response = await _client.get<Map<String, dynamic>>(
        '$_deadLettersPath/$id',
      );
      final data = response.data!;
      await writeCached(
        cache: _cache,
        key: OfflineCacheKeys.webhookDeadLetter(id),
        json: data,
      );
      return WebhookDeadLetter.fromJson(data);
    } on DioException catch (e) {
      final cached = await readCached<WebhookDeadLetter>(
        cache: _cache,
        key: OfflineCacheKeys.webhookDeadLetter(id),
        fromJson: WebhookDeadLetter.fromJson,
      );
      if (cached != null) return cached;
      throw ApiError.fromDioException(e);
    }
  }

  Future<WebhookOverviewMetrics> getOverview() async {
    final deliveries = await listDeliveries(limit: 200);
    final deadLetters = await listDeadLetters(limit: 200);
    final metrics = computeOverviewMetrics(
      deliveries: deliveries,
      deadLetters: deadLetters,
    );
    await writeCached(
      cache: _cache,
      key: OfflineCacheKeys.webhookOverview,
      json: {
        'totalDeliveries': metrics.totalDeliveries,
        'successRate': metrics.successRate,
        'failureRate': metrics.failureRate,
        'retryCount': metrics.retryCount,
        'deadLetterCount': metrics.deadLetterCount,
        'recentFailures': metrics.recentFailures.map((e) => e.toJson()).toList(),
        'recentDeliveries':
            metrics.recentDeliveries.map((e) => e.toJson()).toList(),
      },
    );
    return metrics;
  }
}

final webhooksRepositoryProvider = Provider<WebhooksRepository>(
  (ref) => WebhooksRepository(
    ref.watch(baseApiClientProvider),
    ref.watch(offlineCacheProvider),
  ),
);
