import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/api/base_api_client.dart';
import 'package:ashraak_mobile/core/offline/cached_repository_mixin.dart';
import 'package:ashraak_mobile/core/offline/offline_cache.dart';
import 'package:ashraak_mobile/core/offline/offline_cache_keys.dart';
import 'package:ashraak_mobile/core/offline/offline_providers.dart';
import 'package:ashraak_mobile/features/profile/models/profile_models.dart';
import 'package:ashraak_mobile/shared/errors/api_error.dart';

class ProfileRepository with CachedRepositoryMixin {
  ProfileRepository(this._client, this._cache);

  final BaseApiClient _client;
  final OfflineCache _cache;

  Future<UserProfile> getProfile(String userId) async {
    try {
      final response = await _client.get<Map<String, dynamic>>('/users/$userId');
      final data = response.data!;
      await writeCached(
        cache: _cache,
        key: OfflineCacheKeys.profile(userId),
        json: data,
      );
      return UserProfile.fromJson(data);
    } on DioException catch (e) {
      final cached = await readCached<UserProfile>(
        cache: _cache,
        key: OfflineCacheKeys.profile(userId),
        fromJson: UserProfile.fromJson,
      );
      if (cached != null) return cached;
      throw ApiError.fromDioException(e);
    }
  }

  Future<TenantInfo> getCurrentTenant({String? tenantIdHint}) async {
    try {
      final response = await _client.get<Map<String, dynamic>>('/tenants/current');
      final data = response.data!;
      final tenantId = data['tenantId'] as String;
      await writeCached(
        cache: _cache,
        key: OfflineCacheKeys.tenant(tenantId),
        json: data,
      );
      return TenantInfo.fromJson(data);
    } on DioException catch (e) {
      if (tenantIdHint != null) {
        final cached = await readCached<TenantInfo>(
          cache: _cache,
          key: OfflineCacheKeys.tenant(tenantIdHint),
          fromJson: TenantInfo.fromJson,
        );
        if (cached != null) return cached;
      }
      throw ApiError.fromDioException(e);
    }
  }
}

final profileRepositoryProvider = Provider<ProfileRepository>(
  (ref) => ProfileRepository(
    ref.watch(baseApiClientProvider),
    ref.watch(offlineCacheProvider),
  ),
);
