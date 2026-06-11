import 'package:hive_flutter/hive_flutter.dart';

import 'package:ashraak_mobile/core/offline/offline_cache.dart';

class HiveOfflineCache implements OfflineCache {
  static const _boxName = 'ashraak_offline_cache';
  Box<String>? _box;

  @override
  Future<void> initialize() async {
    await Hive.initFlutter();
    _box = await Hive.openBox<String>(_boxName);
  }

  Box<String> get _safeBox {
    final box = _box;
    if (box == null) throw StateError('HiveOfflineCache not initialized');
    return box;
  }

  @override
  Future<void> put(String key, String jsonValue) async {
    await _safeBox.put(key, jsonValue);
  }

  @override
  Future<String?> get(String key) async => _safeBox.get(key);

  @override
  Future<void> remove(String key) async => _safeBox.delete(key);

  @override
  Future<DateTime?> lastSyncedAt(String namespace) async {
    final raw = await get('last_sync:$namespace');
    if (raw == null) return null;
    return DateTime.tryParse(raw);
  }

  @override
  Future<void> setLastSyncedAt(String namespace, DateTime value) async {
    await put('last_sync:$namespace', value.toUtc().toIso8601String());
  }
}
