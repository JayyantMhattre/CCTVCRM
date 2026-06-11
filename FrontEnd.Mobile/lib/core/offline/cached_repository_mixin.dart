import 'dart:convert';

import 'package:ashraak_mobile/core/offline/offline_cache.dart';

/// Read-through cache helper for repositories (online-first).
mixin CachedRepositoryMixin {
  Future<T?> readCached<T>({
    required OfflineCache cache,
    required String key,
    required T Function(Map<String, dynamic> json) fromJson,
  }) async {
    final raw = await cache.get(key);
    if (raw == null) return null;
    try {
      return fromJson(jsonDecode(raw) as Map<String, dynamic>);
    } catch (_) {
      return null;
    }
  }

  Future<void> writeCached({
    required OfflineCache cache,
    required String key,
    required Map<String, dynamic> json,
  }) async {
    await cache.put(key, jsonEncode(json));
  }

  Future<List<T>?> readCachedList<T>({
    required OfflineCache cache,
    required String key,
    required T Function(Map<String, dynamic> json) fromJson,
  }) async {
    final raw = await cache.get(key);
    if (raw == null) return null;
    try {
      final decoded = jsonDecode(raw) as Map<String, dynamic>;
      final items = decoded['items'];
      if (items is! List) return null;
      return items
          .whereType<Map<String, dynamic>>()
          .map(fromJson)
          .toList();
    } catch (_) {
      return null;
    }
  }
}
