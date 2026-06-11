/// Central route path constants — mirror web route names where applicable.
abstract final class RoutePaths {
  static const splash = '/';
  static const unauthorized = '/unauthorized';
  static const home = '/home';
  static const profile = '/profile';
  static const files = '/files';
  static const notificationPreferences = '/notifications/preferences';
  static const sessions = '/sessions';
  static const tenantSettings = '/settings';
  static const audit = '/audit';
  static const biometricGate = '/biometric-gate';
  static const webhooks = '/webhooks';
  static const webhookDeliveries = '/webhooks/deliveries';
  static const webhookDeadLetters = '/webhooks/deadletters';
  static const apiKeys = '/api-keys';

  static String filePreview(String fileId) => '/files/$fileId/preview';
  static String webhookDeliveryDetail(String id) => '/webhooks/deliveries/$id';
  static String webhookDeadLetterDetail(String id) => '/webhooks/deadletters/$id';
  static String apiKeyDetail(String id) => '/api-keys/$id';
}
