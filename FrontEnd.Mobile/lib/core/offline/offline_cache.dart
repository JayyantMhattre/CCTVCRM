/// Vendor-neutral offline read cache.
abstract class OfflineCache {
  Future<void> initialize();

  Future<void> put(String key, String jsonValue);

  Future<String?> get(String key);

  Future<void> remove(String key);

  Future<DateTime?> lastSyncedAt(String namespace);

  Future<void> setLastSyncedAt(String namespace, DateTime value);
}
