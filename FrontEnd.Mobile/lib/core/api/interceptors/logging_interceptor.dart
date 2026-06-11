import 'package:dio/dio.dart';

import 'package:ashraak_mobile/core/logging/app_logger.dart';

class LoggingInterceptor extends Interceptor {
  LoggingInterceptor(this._logger, {required this.enabled});

  final AppLogger _logger;
  final bool enabled;

  @override
  void onResponse(Response<dynamic> response, ResponseInterceptorHandler handler) {
    if (enabled) {
      _logger.debug('HTTP response', {
        'status': response.statusCode,
        'path': response.requestOptions.uri.path,
      });
    }
    handler.next(response);
  }
}
