import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/api/base_api_client.dart'; // baseApiClientProvider
import 'package:ashraak_mobile/core/auth/current_user.dart';
import 'package:ashraak_mobile/core/feature_flags/feature_flag_keys.dart';
import 'package:ashraak_mobile/core/feature_flags/feature_flag_service.dart';
import 'package:ashraak_mobile/core/offline/offline_cache.dart';
import 'package:ashraak_mobile/core/offline/offline_providers.dart';

const _cacheKeyGlobal = 'feature_flags:global';
const _cacheKeyTenants = 'feature_flags:tenants';

/// Default flags — mirrors backend foundation defaults until remote API exists.
const defaultMobileFlags = <String, bool>{
  MobileFeatureFlags.pushNotifications: true,
  MobileFeatureFlags.biometrics: true,
  MobileFeatureFlags.offlineCache: true,
  MobileFeatureFlags.deepLinks: true,
  MobileFeatureFlags.files: true,
  MobileFeatureFlags.betaFeatures: false,
};

/// Remote + cached + tenant-aware feature flags.
class MobileFeatureFlagProvider implements FeatureFlagService {
  MobileFeatureFlagProvider({
    required this.cache,
    required this.apiClient,
    required this.currentUser,
  });

  final OfflineCache cache;
  final BaseApiClient apiClient;
  final CurrentUser? currentUser;

  Map<String, bool> _global = Map.from(defaultMobileFlags);
  Map<String, Map<String, bool>> _tenantOverrides = {};

  Future<void> initialize() async {
    await _loadFromCache();
    await refreshRemote();
  }

  @override
  Future<bool> isEnabled(String flag, {bool defaultValue = false}) async {
    final tenantId = currentUser?.tenantId;
    if (tenantId != null) {
      final tenantFlags = _tenantOverrides[tenantId];
      if (tenantFlags != null && tenantFlags.containsKey(flag)) {
        return tenantFlags[flag]!;
      }
    }
    return _global[flag] ?? defaultValue;
  }

  Future<void> refreshRemote() async {
    try {
      // Future host endpoint — graceful fallback when not yet exposed.
      final response = await apiClient.get<Map<String, dynamic>>('/platform/feature-flags');
      final data = response.data;
      if (data == null) return;

      final flags = (data['flags'] as Map<String, dynamic>?)
          ?.map((k, v) => MapEntry(k, v == true))
          ?? {};
      final overrides = <String, Map<String, bool>>{};
      final rawOverrides = data['tenantOverrides'] as Map<String, dynamic>?;
      if (rawOverrides != null) {
        for (final entry in rawOverrides.entries) {
          overrides[entry.key] = (entry.value as Map<String, dynamic>)
              .map((k, v) => MapEntry(k, v == true));
        }
      }

      _global = {...defaultMobileFlags, ...flags};
      _tenantOverrides = overrides;
      await _persistCache();
    } on DioException catch (e) {
      if (e.response?.statusCode == 404) return;
      rethrow;
    }
  }

  Future<void> _loadFromCache() async {
    final globalRaw = await cache.get(_cacheKeyGlobal);
    if (globalRaw != null) {
      final decoded = jsonDecode(globalRaw) as Map<String, dynamic>;
      _global = decoded.map((k, v) => MapEntry(k, v == true));
    }

    final tenantsRaw = await cache.get(_cacheKeyTenants);
    if (tenantsRaw != null) {
      final decoded = jsonDecode(tenantsRaw) as Map<String, dynamic>;
      _tenantOverrides = decoded.map(
        (tenantId, flags) => MapEntry(
          tenantId,
          (flags as Map<String, dynamic>).map((k, v) => MapEntry(k, v == true)),
        ),
      );
    }
  }

  Future<void> _persistCache() async {
    await cache.put(_cacheKeyGlobal, jsonEncode(_global));
    await cache.put(_cacheKeyTenants, jsonEncode(_tenantOverrides));
  }
}

final mobileFeatureFlagProviderProvider = Provider<MobileFeatureFlagProvider>((ref) {
  return MobileFeatureFlagProvider(
    cache: ref.watch(offlineCacheProvider),
    apiClient: ref.watch(baseApiClientProvider),
    currentUser: ref.watch(currentUserProvider),
  );
});

final featureFlagServiceProvider = Provider<FeatureFlagService>(
  (ref) => ref.watch(mobileFeatureFlagProviderProvider),
);

final featureFlagEnabledProvider = FutureProvider.family<bool, String>((ref, flag) async {
  final service = ref.watch(featureFlagServiceProvider);
  return service.isEnabled(flag);
});
