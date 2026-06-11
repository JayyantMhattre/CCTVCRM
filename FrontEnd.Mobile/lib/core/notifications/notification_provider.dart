/// Vendor-neutral push notification contract.
abstract class PushNotificationProvider {
  Future<void> initialize();

  Future<bool> requestPermissions();

  Future<String?> getDeviceToken();

  /// Stream of notification tap payloads for deep-link routing.
  Stream<Map<String, String>> get onNotificationOpened;

  /// Stream of foreground message payloads.
  Stream<Map<String, String>> get onForegroundMessage;

  Future<void> dispose();
}

/// Local notification display (foreground / scheduled).
abstract class LocalNotificationProvider {
  Future<void> initialize();

  Future<void> show({
    required int id,
    required String title,
    required String body,
    Map<String, String>? payload,
  });

  Future<void> cancelAll();
}
