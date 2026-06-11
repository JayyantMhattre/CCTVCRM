import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:logger/logger.dart';

import 'package:ashraak_mobile/app/providers.dart';
import 'package:ashraak_mobile/core/environment/environment.dart';

/// Structured logging — environment-aware, no vendor lock-in.
class AppLogger {
  AppLogger({required AppEnvironment environment})
      : _logger = Logger(
          level: environment == AppEnvironment.prod ? Level.warning : Level.debug,
          printer: PrettyPrinter(
            methodCount: 0,
            errorMethodCount: 5,
            lineLength: 100,
            colors: false,
            printEmojis: false,
          ),
        );

  final Logger _logger;

  void debug(String message, [Map<String, Object?>? context]) =>
      _logger.d(_format(message, context));

  void info(String message, [Map<String, Object?>? context]) =>
      _logger.i(_format(message, context));

  void warning(String message, [Map<String, Object?>? context]) =>
      _logger.w(_format(message, context));

  void error(
    String message, {
    Map<String, Object?>? context,
    Object? error,
    StackTrace? stackTrace,
  }) {
    _logger.e(_format(message, context), error: error, stackTrace: stackTrace);
  }

  String _format(String message, Map<String, Object?>? context) {
    if (context == null || context.isEmpty) return message;
    return '$message | $context';
  }
}

final appLoggerProvider = Provider<AppLogger>(
  (ref) => AppLogger(environment: ref.watch(appEnvironmentProvider)),
);
