import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/analytics/analytics_service.dart';
import 'package:ashraak_mobile/core/analytics/logging_analytics_service.dart';
import 'package:ashraak_mobile/core/analytics/noop_analytics_service.dart';
import 'package:ashraak_mobile/core/logging/app_logger.dart';

final analyticsServiceProvider = Provider<AnalyticsService>((ref) {
  const useLogging = bool.fromEnvironment('ANALYTICS_LOG', defaultValue: true);
  if (useLogging && !kReleaseMode) {
    return LoggingAnalyticsService(ref.watch(appLoggerProvider));
  }
  return NoOpAnalyticsService();
});
