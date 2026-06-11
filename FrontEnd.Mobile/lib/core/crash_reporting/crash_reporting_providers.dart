import 'dart:async';

import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/crash_reporting/crash_reporting_service.dart';
import 'package:ashraak_mobile/core/crash_reporting/logging_crash_reporter.dart';
import 'package:ashraak_mobile/core/crash_reporting/noop_crash_reporter.dart';
import 'package:ashraak_mobile/core/logging/app_logger.dart';

final crashReportingServiceProvider = Provider<CrashReportingService>((ref) {
  const useLogging = bool.fromEnvironment('CRASH_REPORTING_LOG', defaultValue: true);
  if (useLogging && !kReleaseMode) {
    return LoggingCrashReporter(ref.watch(appLoggerProvider));
  }
  return NoOpCrashReporter();
});

Future<void> initializeCrashReporting(WidgetRef ref) async {
  final reporter = ref.read(crashReportingServiceProvider);
  await reporter.initialize();

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

Future<void> reportDioError(CrashReportingService reporter, DioException error) {
  return reporter.recordError(
    error,
    stackTrace: error.stackTrace,
    context: {
      'path': error.requestOptions.uri.path,
      'status': error.response?.statusCode,
      'source': 'Dio',
    },
  );
}
