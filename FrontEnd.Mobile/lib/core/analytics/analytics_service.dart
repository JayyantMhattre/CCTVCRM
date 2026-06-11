/// Vendor-neutral product analytics.
abstract class AnalyticsService {
  Future<void> initialize();

  Future<void> setUserContext({
    String? userId,
    String? tenantId,
    List<String>? roles,
  });

  Future<void> trackScreen(String screenName, {Map<String, Object?>? properties});

  Future<void> trackEvent(String name, {Map<String, Object?>? properties});
}
