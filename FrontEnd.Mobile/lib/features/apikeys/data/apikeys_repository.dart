import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/api/base_api_client.dart';
import 'package:ashraak_mobile/core/offline/cached_repository_mixin.dart';
import 'package:ashraak_mobile/core/offline/offline_cache.dart';
import 'package:ashraak_mobile/core/offline/offline_cache_keys.dart';
import 'package:ashraak_mobile/core/offline/offline_providers.dart';
import 'package:ashraak_mobile/features/apikeys/models/apikey_models.dart';
import 'package:ashraak_mobile/shared/errors/api_error.dart';

class ApiKeysRepository with CachedRepositoryMixin {
  ApiKeysRepository(this._client, this._cache);

  final BaseApiClient _client;
  final OfflineCache _cache;

  static const _basePath = '/api-keys';

  Future<List<ApiKey>> listApiKeys() async {
    try {
      final response = await _client.get<List<dynamic>>(_basePath);
      final items = parseApiKeysList(response.data);
      await writeCached(
        cache: _cache,
        key: OfflineCacheKeys.apiKeys,
        json: {'items': items.map((e) => e.toJson()).toList()},
      );
      return items;
    } on DioException catch (e) {
      final cached = await readCachedList<ApiKey>(
        cache: _cache,
        key: OfflineCacheKeys.apiKeys,
        fromJson: ApiKey.fromJson,
      );
      if (cached != null) return cached;
      throw ApiError.fromDioException(e);
    }
  }

  Future<ApiKey> getApiKey(String id) async {
    try {
      final response = await _client.get<Map<String, dynamic>>('$_basePath/$id');
      final data = response.data!;
      await writeCached(
        cache: _cache,
        key: OfflineCacheKeys.apiKey(id),
        json: data,
      );
      return ApiKey.fromJson(data);
    } on DioException catch (e) {
      final cached = await readCached<ApiKey>(
        cache: _cache,
        key: OfflineCacheKeys.apiKey(id),
        fromJson: ApiKey.fromJson,
      );
      if (cached != null) return cached;
      throw ApiError.fromDioException(e);
    }
  }
}

final apiKeysRepositoryProvider = Provider<ApiKeysRepository>(
  (ref) => ApiKeysRepository(
    ref.watch(baseApiClientProvider),
    ref.watch(offlineCacheProvider),
  ),
);
