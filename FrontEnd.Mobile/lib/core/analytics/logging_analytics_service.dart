import 'package:ashraak_mobile/core/analytics/analytics_service.dart';
import 'package:ashraak_mobile/core/logging/app_logger.dart';

/// Local analytics sink for dev/CI — swap for Firebase/Mixpanel/PostHog via ADR.
class LoggingAnalyticsService implements AnalyticsService {
  LoggingAnalyticsService(this._logger);

  final AppLogger _logger;

  @override
  Future<void> initialize() async {}

  @override
  Future<void> setUserContext({
    String? userId,
    String? tenantId,
    List<String>? roles,
  }) async {
    _logger.debug('Analytics user context', {
      'userId': userId,
      'tenantId': tenantId,
      'roles': roles,
    });
  }

  @override
  Future<void> trackScreen(String screenName, {Map<String, Object?>? properties}) async {
    _logger.debug('Analytics screen', {'screen': screenName, ...?properties});
  }

  @override
  Future<void> trackEvent(String name, {Map<String, Object?>? properties}) async {
    _logger.debug('Analytics event', {'event': name, ...?properties});
  }
}
