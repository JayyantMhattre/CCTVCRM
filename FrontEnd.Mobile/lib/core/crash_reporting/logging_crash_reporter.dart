import 'package:ashraak_mobile/core/crash_reporting/crash_reporting_service.dart';
import 'package:ashraak_mobile/core/logging/app_logger.dart';

/// Development-friendly reporter — logs locally without external vendor.
class LoggingCrashReporter implements CrashReportingService {
  LoggingCrashReporter(this._logger);

  final AppLogger _logger;

  @override
  Future<void> initialize() async {}

  @override
  Future<void> recordError(
    Object error, {
    StackTrace? stackTrace,
    Map<String, Object?>? context,
    bool fatal = false,
  }) async {
    _logger.error(
      fatal ? 'Fatal error' : 'Captured error',
      context: context,
      error: error,
      stackTrace: stackTrace,
    );
  }

  @override
  Future<void> setUserContext({
    String? userId,
    String? tenantId,
    String? email,
  }) async {
    _logger.info('Crash user context', {
      'userId': userId,
      'tenantId': tenantId,
      'email': email,
    });
  }

  @override
  Future<void> log(String message) async => _logger.info(message);
}
