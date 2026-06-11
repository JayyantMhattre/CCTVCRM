import 'package:ashraak_mobile/core/crash_reporting/crash_reporting_service.dart';

class NoOpCrashReporter implements CrashReportingService {
  @override
  Future<void> initialize() async {}

  @override
  Future<void> recordError(
    Object error, {
    StackTrace? stackTrace,
    Map<String, Object?>? context,
    bool fatal = false,
  }) async {}

  @override
  Future<void> setUserContext({
    String? userId,
    String? tenantId,
    String? email,
  }) async {}

  @override
  Future<void> log(String message) async {}
}
