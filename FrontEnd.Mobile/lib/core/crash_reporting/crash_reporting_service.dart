/// Vendor-neutral crash and error reporting.
abstract class CrashReportingService {
  Future<void> initialize();

  Future<void> recordError(
    Object error, {
    StackTrace? stackTrace,
    Map<String, Object?>? context,
    bool fatal = false,
  });

  Future<void> setUserContext({
    String? userId,
    String? tenantId,
    String? email,
  });

  Future<void> log(String message);
}
