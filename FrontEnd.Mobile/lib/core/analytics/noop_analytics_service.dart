import 'package:ashraak_mobile/core/analytics/analytics_service.dart';

class NoOpAnalyticsService implements AnalyticsService {
  @override
  Future<void> initialize() async {}

  @override
  Future<void> setUserContext({
    String? userId,
    String? tenantId,
    List<String>? roles,
  }) async {}

  @override
  Future<void> trackScreen(String screenName, {Map<String, Object?>? properties}) async {}

  @override
  Future<void> trackEvent(String name, {Map<String, Object?>? properties}) async {}
}
