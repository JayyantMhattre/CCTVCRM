/// Background sync targets — no aggressive polling.
enum SyncTarget {
  profile,
  tenant,
  settings,
  notifications,
  filesMetadata,
  webhooks,
  apiKeys,
}

extension SyncTargetKey on SyncTarget {
  String get namespace => name;
}
