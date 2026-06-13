/// Cache key namespaces for online-first read-through caching.
abstract final class OfflineCacheKeys {
  static String profile(String userId) => 'profile:$userId';
  static String tenant(String tenantId) => 'tenant:$tenantId';
  static String settings(String tenantId) => 'settings:$tenantId';
  static String notificationPrefs(String userId) => 'notification_prefs:$userId';
  static String filesMetadata(String userId) => 'files_metadata:$userId';
  static const webhookOverview = 'webhooks:overview';
  static const webhookDeliveries = 'webhooks:deliveries';
  static String webhookDelivery(String id) => 'webhooks:delivery:$id';
  static const webhookDeadLetters = 'webhooks:deadletters';
  static String webhookDeadLetter(String id) => 'webhooks:deadletter:$id';
  static const apiKeys = 'apikeys:list';
  static String apiKey(String id) => 'apikeys:detail:$id';

  static const syncProfile = 'sync:profile';
  static const syncSettings = 'sync:settings';
  static const syncNotifications = 'sync:notifications';
  static const syncFiles = 'sync:files';
  static const syncTenant = 'sync:tenant';
  static const syncWebhooks = 'sync:webhooks';
  static const syncApiKeys = 'sync:apikeys';

  static const cctvCustomerTickets = 'cctv:customer:tickets';
  static const cctvCustomerInvoices = 'cctv:customer:invoices';
  static const cctvCustomerAmc = 'cctv:customer:amc';
  static const cctvCustomerVisits = 'cctv:customer:visits';
  static const cctvEngineerSchedules = 'cctv:engineer:schedules';
  static const cctvEngineerTickets = 'cctv:engineer:tickets';
  static const cctvEngineerDashboard = 'cctv:engineer:dashboard';
  static const cctvOfflineQueue = 'cctv:offline:queue';
}
