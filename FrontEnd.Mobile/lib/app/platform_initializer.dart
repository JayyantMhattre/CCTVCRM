import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/analytics/analytics_providers.dart';
import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:ashraak_mobile/core/crash_reporting/crash_reporting_providers.dart';
import 'package:ashraak_mobile/core/crash_reporting/crash_reporting_service.dart';
import 'package:ashraak_mobile/core/feature_flags/mobile_feature_flag_provider.dart';
import 'package:ashraak_mobile/core/logging/app_logger.dart';
import 'package:ashraak_mobile/core/navigation/deep_links/deep_link_handler.dart';
import 'package:ashraak_mobile/core/notifications/notification_providers.dart';
import 'package:ashraak_mobile/core/offline/offline_providers.dart';
import 'package:ashraak_mobile/core/sync/sync_providers.dart';

/// One-time production platform initialization (M4).
Future<void> initializePlatform(ProviderContainer container) async {
  final logger = container.read(appLoggerProvider);
  logger.info('Initializing production platform services');

  await container.read(offlineCacheProvider).initialize();
  final crashReporter = container.read(crashReportingServiceProvider);
  await crashReporter.initialize();
  _bindCrashHandlers(crashReporter);
  await container.read(analyticsServiceProvider).initialize();
  await container.read(mobileFeatureFlagProviderProvider).initialize();

  final notifications = container.read(notificationServiceProvider);
  final deepLinks = container.read(deepLinkHandlerProvider);
  await notifications.initialize(
    onTap: (payload) {
      // Navigation bound when router is available in shell.
      logger.info('Notification tap', payload);
    },
  );

  // Start connectivity + resume sync coordinator.
  container.read(backgroundSyncCoordinatorProvider);
  container.read(offlineStatusProvider);

  logger.info('Platform services ready', {
    'offline': true,
    'sync': true,
    'notifications': true,
    'featureFlags': true,
  });

  // Deep links bound from [AppShellPage] when GoRouter is available.
  deepLinks;
}

void _bindCrashHandlers(CrashReportingService reporter) {
  FlutterError.onError = (details) {
    unawaited(
      reporter.recordError(
        details.exception,
        stackTrace: details.stack,
        context: {'source': 'FlutterError'},
        fatal: true,
      ),
    );
  };

  PlatformDispatcher.instance.onError = (error, stack) {
    unawaited(
      reporter.recordError(
        error,
        stackTrace: stack,
        context: {'source': 'PlatformDispatcher'},
        fatal: true,
      ),
    );
    return true;
  };
}
